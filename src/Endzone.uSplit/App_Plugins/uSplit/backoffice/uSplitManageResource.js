angular.module("umbraco.resources").factory("uSplitManageResource",
    function ($http) {
        return {
            getExperiment: function (id) {
                return $http.get("backoffice/uSplit/Manage/GetExperimentAsync/"+id);
            }
        }
    }
);