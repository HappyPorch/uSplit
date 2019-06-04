angular.module("umbraco.resources").factory("uSplitManageResource",
    function ($http) {
        var controllerPath = "backoffice/uSplit/Manage/";
        return {
            getExperiment: function (id, profileId) {
                return $http.get(controllerPath + "GetExperimentAsync/" + id,
                    { params: { profileId: profileId } }
                );
            },
            createExperiment: function (name, nodeId, profileId) {
                var params = { name, profileId };
                if (nodeId) {
                    params.nodeId = nodeId;
                }
                return $http.post(controllerPath + "CreateExperimentAsync",
                    {}, { params: params }
                );
            },
            addVariation: function (name, experimentId, nodeId, profileId) {
                var data = {
                    name, experimentId
                };
                if (nodeId) {
                    data.nodeId = nodeId;
                }
                return $http.post(controllerPath + "AddVariationAsync",
                    data,
                    { params: { profileId: profileId } }
                );
            },
            deleteExperiment: function (id, profileId) {
                return $http.delete(controllerPath + "DeleteExperimentAsync/" + id,
                    { params: { profileId: profileId } }
                );
            },
            deleteVariation: function (experimentId, variationName, profileId) {
                return $http.post(controllerPath + "DeleteVariationAsync",
                    { experimentId, variationName },
                    { params: { profileId: profileId } }
                );
            },
            setSegment: function (experimentId, providerKey, value, profileId) {
                return $http.post(controllerPath + "SetSegmentAsync",
                    { experimentId, providerKey, value },
                    { params: { profileId: profileId } }
                );
            },
            start: function (experimentId, profileId) {
                return $http.post(controllerPath + "StartExperimentAsync/" + experimentId,
                    {},
                    { params: { profileId: profileId } }
                );
            },
            stop: function (experimentId, profileId) {
                return $http.post(controllerPath + "StopExperimentAsync/" + experimentId,
                    {},
                    { params: { profileId: profileId } }
                );
            }
        }
    }
);