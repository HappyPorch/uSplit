angular.module("umbraco").controller("uSplit.abTesting.experimentController",
    function ($scope,
              $routeParams,
              $q,
              notificationsService,
              editorState,
              dialogService,
              contentResource,
              navigationService,
              $location,
              uSplitConfigurationResource,
              uSplitManageResource) {

        $scope.loaded = false;
        //refactor this to a parent controller?
        $scope.apiReady = false;

        var urlParts = decodeURI($routeParams.id).split("?");
        var profileId = $scope.profileId = urlParts[1];
        var experimentId = $scope.experimentId = urlParts[0];
        navigationService.syncTree({ tree: "abtesting", path: [-1, profileId, experimentId], forceReload: false, activate: true });

        $scope.tabs = [{ id: "variations", label: "Variations" }, { id: "debug", label: "Debug" }];

        var noSegmentation = {
            name: "All traffic",
            providerKey: null,
            value: null
        };

        $scope.segmentation = {
            provider: noSegmentation,
            providers: [noSegmentation]
        };

        function addToVariations(addVariationResponse) {
            var variation = addVariationResponse.data;
            $scope.experiment.variations.push(variation);
        }

        function handleDotNetError(error) {
            if (error.status && error.status >= 500) {
                dialogService.ysodDialog(error);
            }
        }

        function handleExperiment(response) {
            $scope.experiment = response.data;

            //segmentation
            var provider = $scope.segmentation.providers.find(function (item) {
                return item.providerKey == $scope.experiment.segmentationProviderKey;
            });
            if (provider != null) {
                provider.value = $scope.experiment.segmentationValue;
            }
            $scope.segmentation.provider = provider;
            editorState.set($scope.experiment);
        }

        var segmentationProvidersUpdate = uSplitConfigurationResource
            .getSegmentationProviders()
            .then(function (response) {
                $scope.segmentation.providers = response.data;
                $scope.segmentation.providers.unshift(noSegmentation);
            }, handleDotNetError);

        $scope.refresh = function () {
            $scope.loaded = false;

            //we need reference data
            segmentationProvidersUpdate.then(function() {
                var statusUpdate = uSplitConfigurationResource.getStatus(profileId)
                    .then(function (response) {
                        $scope.apiReady = response.data === "true";
                    }, handleDotNetError);

                var experimentLoad = uSplitManageResource.getExperiment(experimentId, profileId)
                    .then(handleExperiment, handleDotNetError);

                $q.all([statusUpdate, experimentLoad])
                    .then(function () {
                        $scope.loaded = true;
                    });
            });
        };

        $scope.editContent = function (nodeId) {
            $location.search("");
            $location.path("/content/content/edit/" + nodeId);
        }

        //TODO: add an indicator and possibly disable input?
        $scope.duplicateAsVariation = function () {
            dialogService.contentPicker({
                callback: function (data) {
                    //TODO: check if can be copied (e.g. doctype is amongst allowed chlidren)
                    contentResource.copy({ id: $scope.experiment.nodeId, parentId: data.id }).then(function (path) {
                            var id = path.split(',').pop();
                            uSplitManageResource.addVariation(experimentId, id, profileId).then(addToVariations);
                        },
                        function(response) {
                            if (angular.isArray(response.data.notifications)) {
                                for (var i = 0; i < response.data.notifications.length; i++) {
                                    notificationsService.showNotification(response.data.notifications[i]);
                                }
                            }
                        }
                    );
                }
            });
        }

        //TODO: add an indicator and possibly disable input?
        $scope.addExistingAsVariation = function () {
            dialogService.contentPicker({
                callback: function (data) {
                    //TODO: check for doctype
                    var id = data.id;
                    uSplitManageResource.addVariation(experimentId, id, profileId).then(addToVariations);
                }
            });
        }

        //TODO: add an indicator and possibly disable input?
        $scope.deleteVariation = function (variationName) {
            uSplitManageResource.deleteVariation(experimentId, variationName, profileId).then(function () {
                $scope.experiment.variations = $scope.experiment.variations.filter(function (v) {
                    return v.googleName !== variationName;
                });
            });
        };

        //TODO: add an indicator and possibly disable input?
        $scope.start = function () {
            uSplitManageResource.start(experimentId, profileId).then(function (response) {
                handleExperiment(response);
            }, handleDotNetError);
        };

        //TODO: add an indicator and possibly disable input?
        $scope.stop = function () {
            uSplitManageResource.stop(experimentId, profileId).then(function (response) {
                handleExperiment(response);
            }, handleDotNetError);
        };

        $scope.$watch(function () {
            return $scope.segmentation.provider;
        }, function () {
            if (!$scope.experiment)
                return; //no data loaded yet

            var provider = $scope.segmentation.provider;
            if (provider.providerKey == $scope.experiment.segmentationProviderKey &&
                provider.value == $scope.experiment.segmentationValue)
                return; //no change, initial load

            //update
            uSplitManageResource.setSegment(experimentId, provider.providerKey, provider.value, profileId)
                .then(function () {
                    //local update to correctly detect new changes
                    $scope.experiment.segmentationProviderKey = provider.providerKey;
                    $scope.experiment.segmentationValue = provider.value;
                });
        }, true);

        $scope.refresh();
    });
