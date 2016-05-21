angular.module("umbraco").controller("uSplit.abTesting.experimentController",
    function ($scope, $routeParams, $q, uSplitGoogleAuthResource, uSplitManageResource) {
        function parseExperimentData(experiment) {
            var originalPage = experiment.variations.find(function (variation) {
                return variation.name == "Original";
            });

            var variations = experiment.variations.filter(function (variation) {
                return variation.name != "Original";
            })

            return {
                name: experiment.name,
                originalPage,
                variations
            };
        }

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
                    $scope.experimentData = response.data;
                    $scope.experiment = parseExperimentData($scope.experimentData);
                });

            $q.all([statusUpdate, experimentLoad])
                .then(function() {
                    $scope.loaded = true;
                });
        };

        $scope.refresh();
    });
