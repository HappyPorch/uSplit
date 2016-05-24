angular.module("umbraco").controller("uSplit.abTesting.experimentController",
    function ($scope, $routeParams, $q, dialogService, contentResource, $location, uSplitGoogleAuthResource, uSplitManageResource) {
        $scope.loaded = false;
        //refactor this to a parent controller?
        $scope.apiReady = false;

        var experimentId = $scope.experimentId = $routeParams.id;

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

        $scope.addExistingAsVariation = function () {
            dialogService.contentPicker({
                callback: function (data) {
                    //TODO: check for doctype
                    var id = data.id;
                    uSplitManageResource.addVariation(experimentId, id).then(addToVariations);
                }
            });
        }

        $scope.refresh();
    });
