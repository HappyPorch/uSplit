angular.module("umbraco.resources").factory("uSplitGoogleAuthResource",
    function ($http) {
        return {
            getStatus: function () {
                return $http.get("backoffice/uSplit/GoogleApi/Status");
            },

            //reauthorize: function (documentTypeId) {
            //    return $http.get("backoffice/uSplit/GoogleApi/DoAuth", {
            //        params: {
            //            contentTypeId: documentTypeId
            //        }
            //    });
            //}
        }
    }
);