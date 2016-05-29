angular.module("umbraco.resources").factory("uSplitGoogleAuthResource",
    function ($http) {
        return {
            getStatus: function () {
                return $http.get("backoffice/uSplit/GoogleApi/Status");
            },
            checkAccess: function () {
                return $http.get("backoffice/uSplit/GoogleApi/checkAccess");
            }
        }
    }
);