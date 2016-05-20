angular.module("umbraco").controller("uSplit.abTesting.configurationController",
    function ($scope, uSplitGoogleAuthResource) {

        $scope.loaded = false;
        $scope.status = false;

        $scope.tabs = [{ id: "configuration", label: "Configuration" }];

        $scope.reauthorize = function () {
            window.location = 'backoffice/usplit/GoogleAuth/ReauthorizeAsync?originalUrl=' + encodeURIComponent(window.location);
        }

        $scope.refresh = function () {
            $scope.loaded = false;

            uSplitGoogleAuthResource.getStatus().then(function (response) {
                $scope.status = response.data === "true";
                $scope.loaded = true;
            });
        };

        $scope.refresh();
    });
