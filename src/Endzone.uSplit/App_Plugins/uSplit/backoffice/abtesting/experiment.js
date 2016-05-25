angular.module("umbraco").controller("uSplit.abTesting.experimentController",
    function ($scope,
        $routeParams,
        $q,
        editorState,
        dialogService,
        contentResource,
        navigationService,
        $location,
        uSplitGoogleAuthResource,
        uSplitManageResource) {
        $scope.loaded = false;
        //refactor this to a parent controller?
        $scope.apiReady = false;

        var experimentId = $scope.experimentId = $routeParams.id;
        navigationService.syncTree({ tree: "abtesting", path: [-1, experimentId], forceReload: false, activate: true });

        $scope.tabs = [{ id: "variations", label: "Variations" }, { id: "debug", label: "Debug" }];

        $scope.refresh = function () {
            $scope.loaded = false;

            var statusUpdate = uSplitGoogleAuthResource.getStatus()
                .then(function (response) {
                    $scope.apiReady = response.data === "true";
                });

            var experimentLoad = uSplitManageResource.getExperiment(experimentId)
                .then(function(response) {
                    $scope.experiment = response.data;
                    editorState.set($scope.experiment);
                });

            $q.all([statusUpdate, experimentLoad])
                .then(function() {
                    $scope.loaded = true;
                });
        };

        $scope.editContent = function (nodeId) {
            $location.search("");
            $location.path("/content/content/edit/" + nodeId);
        }

        function addToVariations(addVariationResponse) {
            var variation = addVariationResponse.data;
            $scope.experiment.variations.push(variation);
        }

        //TODO: handle errors
        $scope.duplicateAsVariation = function () {
            dialogService.contentPicker({
                callback: function (data) {
                    //TODO: check if can be copied (e.g. doctype is amongst allowed chlidren)
                    contentResource.copy({ id: $scope.experiment.nodeId, parentId: data.id }).then(function (path) {
                        var id = path.split(',').pop();
                        uSplitManageResource.addVariation(experimentId, id).then(addToVariations);
                    });
                }
            });
        }

        //TODO: handle errors
        $scope.addExistingAsVariation = function () {
            dialogService.contentPicker({
                callback: function (data) {
                    //TODO: check for doctype
                    var id = data.id;
                    uSplitManageResource.addVariation(experimentId, id).then(addToVariations);
                }
            });
        }

        //TODO: handle errors
        $scope.deleteVariation = function (variationName) {
            uSplitManageResource.deleteVariation(experimentId, variationName).then(function () {
                $scope.experiment.variations = $scope.experiment.variations.filter(function(v) {
                    return v.googleName !== variationName;
                });
            });
        };

        $scope.refresh();
    });
