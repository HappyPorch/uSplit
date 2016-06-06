angular.module("umbraco.resources").factory("uSplitConfigurationResource",
    function ($http) {
        return {
            getStatus: function () {
                return $http.get("backoffice/uSplit/Configuration/Status");
            },
            checkAccess: function () {
                return $http.get("backoffice/uSplit/Configuration/CheckAccess");
            },
            getLicenseInfo: function () {
                return $http.get("backoffice/uSplit/Configuration/License");
            }
        }
    }
);