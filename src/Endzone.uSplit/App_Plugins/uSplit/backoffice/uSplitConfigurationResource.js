angular.module("umbraco.resources").factory("uSplitConfigurationResource",
    function ($http) {
        var controllerPath = "backoffice/uSplit/Configuration/";
        return {
            getStatus: function (profileId) {
                console.log(profileId);
                return $http.get(controllerPath+ "Status", {
                    params: {profileId: profileId}
                });
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