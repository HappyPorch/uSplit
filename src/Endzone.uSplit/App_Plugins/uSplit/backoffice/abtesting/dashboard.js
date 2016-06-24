angular.module("umbraco").controller("uSplit.abTesting.configurationController",
    function ($scope, $q, uSplitConfigurationResource) {

        $scope.loaded = false;
        $scope.connected = false;
        $scope.hasAccess = false;

        $scope.tabs = [
            { id: "configuration", label: "Configuration" },
            { id: "instructions", label: "Instructions" }
        ];

        $scope.reauthorize = function () {
            window.location = 'backoffice/usplit/GoogleAuth/ReauthorizeAsync?originalUrl=' + encodeURIComponent(window.location);
        }

        $scope.refresh = function () {
            $scope.loaded = false;

            var statusUpdate = uSplitConfigurationResource.getStatus().then(function (response) {
                $scope.connected = response.data === "true";
            });

            var accessUpdate = uSplitConfigurationResource.checkAccess().then(function (response) {
                $scope.hasAccess = response.data.hasAccess;
                $scope.message = response.data.error;
            });

            $q.all(statusUpdate, accessUpdate)
                .then(function() {
                    $scope.loaded = true;
                });
        };

        $scope.refresh();
    });
