angular.module("umbraco")
    .controller("uSplit.PersonalisationGroups.abTesting.segmentsController",
        function($scope, dataTypeResource) {
            //we want to embed a personalisation group picker shipped with Personalisation Groups
            var groupPickerName = "Personalisation group picker"; //the name of the Picker, hope no-one changed it!
            //todo: have a fail switch - just allow the user to pick any content based on a doctype filter
            var picker = dataTypeResource.getByName(groupPickerName);
            picker.then(function(dataType) {
                $scope.segmentPicker = {
                    id: 0,
                    label: '',
                    alias: "group",
                    description: null,
                    view: 'contentpicker',
                    config: {},
                    hideLabel: true,
                    validation: {
                        mandatory: false,
                        pattern: null
                    },
                    value: $scope.segmentation.provider.value,
                    editor: dataType.selectedEditor
                };

                dataType.preValues.forEach(function(item) {
                    $scope.segmentPicker.config[item.key] = item.value;
                });

                $scope.$watch(function () {
                    return $scope.segmentPicker.value;
                }, function (value) {
                    $scope.segmentation.provider.value = value;
                });
            });
        }
);