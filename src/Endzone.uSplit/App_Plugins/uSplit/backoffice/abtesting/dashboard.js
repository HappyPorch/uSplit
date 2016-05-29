angular.module("umbraco").controller("uSplit.abTesting.configurationController",
    function ($scope, uSplitGoogleAuthResource, dialogService) {

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

            uSplitGoogleAuthResource.getStatus().then(function (response) {
                $scope.connected = response.data === "true";
                $scope.loaded = true;
            });

            uSplitGoogleAuthResource.checkAccess().then(function (response) {
                $scope.hasAccess = response.data.hasAccess;
                $scope.message = response.data.error;
            });
        };

        $scope.refresh();
    });
