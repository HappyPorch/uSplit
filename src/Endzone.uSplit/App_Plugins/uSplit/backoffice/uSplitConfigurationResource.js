angular.module("umbraco.resources").factory("uSplitConfigurationResource",
    function ($http) {
        var controllerPath = "backoffice/uSplit/Configuration/";
        return {
            getStatus: function () {
                return $http.get(controllerPath+ "Status");
            },
            checkAccess: function () {
                return $http.get(controllerPath+ "CheckAccess");
            },
            getSegmentationProviders: function() {
                return $http.get(controllerPath+ "GetSegmentationProviders");
            }
        }
    }
);