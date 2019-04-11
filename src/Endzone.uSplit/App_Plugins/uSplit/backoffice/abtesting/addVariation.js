angular.module("umbraco").controller("uSplit.abTesting.addVariationController",
    function ($scope)
    {
        if (typeof $scope.dialogData === 'object')
            $scope.dialogData = null;

        $scope.editorModel = {
            view: 'textbox',
            value: $scope.dialogData
        };
    });