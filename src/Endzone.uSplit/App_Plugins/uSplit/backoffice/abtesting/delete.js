angular.module("umbraco")
    .controller("uSplit.abTesting.deleteController",
        function ($scope, uSplitManageResource, treeService, navigationService, editorState, $location, dialogService, notificationsService) {

            $scope.performDelete = function () {

                //mark it for deletion (used in the UI)
                var profileId = $scope.currentNode.parentId;
                $scope.currentNode.loading = true;

                uSplitManageResource.deleteExperiment($scope.currentNode.id, profileId).then(function () {
                    $scope.currentNode.loading = false;

                    var rootNode = treeService.getTreeRoot($scope.currentNode);
                    treeService.removeNode($scope.currentNode);

                    //if the current edited item is the same one as we're deleting, we need to navigate elsewhere
                    if (editorState.current && editorState.current.googleId == $scope.currentNode.id) {
                        $location.path(rootNode.routePath);
                    }

                    navigationService.hideMenu();
                }, function (err) {

                    $scope.currentNode.loading = false;

                    //check if response is ysod
                    if (err.status && err.status >= 500) {
                        dialogService.ysodDialog(err);
                    }

                    if (err.data && angular.isArray(err.data.notifications)) {
                        for (var i = 0; i < err.data.notifications.length; i++) {
                            notificationsService.showNotification(err.data.notifications[i]);
                        }
                    }
                });

            };

            $scope.cancel = function () {
                navigationService.hideDialog();
            };
        }
);