angular.module("umbraco.resources").factory("uSplitManageResource",
    function ($http) {
        var controllerPath = "backoffice/uSplit/Manage/";
        return {
            getExperiment: function (id) {
                return $http.get(controllerPath + "GetExperimentAsync/" + id);
            },
            createExperiment: function(id) {
                return $http.get(controllerPath + "CreateExperimentAsync/" + id);
            },
            addVariation: function(experimentId, nodeId) {
                return $http.post(controllerPath + "AddVariationAsync/",
                {
                    experimentId,
                    nodeId
                });
            },
            deleteExperiment: function(id) {
                return $http.delete(controllerPath + "DeleteExperimentAsync/" + id);
            },
            deleteVariation: function (experimentId, variationName) {
                return $http.post(controllerPath + "DeleteVariationAsync/",
                {
                    experimentId,
                    variationName
                });
            },
            setSegment: function (experimentId, providerKey, value) {
                return $http.post(controllerPath + "SetSegmentAsync/", {
                    experimentId,
                    providerKey,
                    value
                });
            },
            start: function (experimentId) {
                return $http.post(controllerPath + "StartExperimentAsync/" + experimentId);
            },
            stop: function (experimentId) {
                return $http.post(controllerPath + "StopExperimentAsync/" + experimentId);
            }
        }
    }
);