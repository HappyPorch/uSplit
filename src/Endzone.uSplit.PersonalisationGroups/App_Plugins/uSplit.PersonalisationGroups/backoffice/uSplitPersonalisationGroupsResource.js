angular.module("umbraco.resources").factory("uSplitPersonalisationGroupsResource",
    function ($http) {
        return {
            getSegments: function () {
                return $http.get("backoffice/uSplitPersonalisationGroups/Segments");
            }
        }
    }
);