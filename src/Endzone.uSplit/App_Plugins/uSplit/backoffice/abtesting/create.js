angular.module("umbraco")
    .controller("uSplit.abTesting.createController",
        function($scope,
            eventsService,
            contentResource,
            navigationService,
            appState,
            treeService,
            $location,
            localizationService,
            notificationsService,
            uSplitManageResource) {
        
            var profileId = $scope.currentAction.metaData.profileId;

            var searchText = "Search...";
            localizationService.localize("general_search")
                .then(function(value) {
                    searchText = value + "...";
                });

            $scope.dialogTreeEventHandler = $({});
            $scope.busy = false;
            $scope.searchInfo = {
                searchFromId: null,
                searchFromName: null,
                showSearch: false,
                results: [],
                selectedSearchResults: []
            }

            function nodeSelectHandler(ev, args) {
                args.event.preventDefault();
                args.event.stopPropagation();

                if (args.node.metaData.listViewNode) {
                    //check if list view 'search' node was selected

                    $scope.searchInfo.showSearch = true;
                    $scope.searchInfo.searchFromId = args.node.metaData.listViewNode.id;
                    $scope.searchInfo.searchFromName = args.node.metaData.listViewNode.name;
                } else {
                    if ($scope.target) {
                        //un-select if there's a current one selected
                        $scope.target.selected = false;
                    }

                    $scope.target = args.node;
                    $scope.target.selected = true;
                }

            }

            function nodeExpandedHandler(ev, args) {
                if (angular.isArray(args.children)) {

                    //iterate children
                    _.each(args.children,
                        function(child) {
                            //check if any of the items are list views, if so we need to add a custom
                            // child: A node to activate the search
                            if (child.metaData.isContainer) {
                                child.hasChildren = true;
                                child.children = [
                                    {
                                        level: child.level + 1,
                                        hasChildren: false,
                                        name: searchText,
                                        metaData: {
                                            listViewNode: child,
                                        },
                                        cssClass: "icon umb-tree-icon sprTree icon-search",
                                        cssClasses: ["not-published"]
                                    }
                                ];
                            }
                        });
                }
            }

            $scope.hideSearch = function() {
                $scope.searchInfo.showSearch = false;
                $scope.searchInfo.searchFromId = null;
                $scope.searchInfo.searchFromName = null;
                $scope.searchInfo.results = [];
            }

            // method to select a search result
            $scope.selectResult = function(evt, result) {
                result.selected = result.selected === true ? false : true;
                nodeSelectHandler(evt, { event: evt, node: result });
            };

            //callback when there are search results
            $scope.onSearchResults = function(results) {
                $scope.searchInfo.results = results;
                $scope.searchInfo.showSearch = true;
            };

            $scope.editExperiment = function () {
                $scope.nav.hideDialog();
                $location.search("");
                $location.path("content/abtesting/experiment/" + $scope.experiment.id + encodeURI("?" + profileId));
            }

            $scope.create = function() {

                $scope.busy = true;
                $scope.error = false;

                uSplitManageResource.createExperiment($scope.target.id, profileId)
                    .then(function (createResponse) {   
                        $scope.error = false;
                        $scope.success = true;
                        $scope.busy = false;

                        var experiment = $scope.experiment = createResponse.data;

                        navigationService.syncTree({ tree: "abtesting", path: [-1, experiment.id], forceReload: true, activate: false });
                    }, function (err) {
                        $scope.success = false;
                        $scope.error = err;
                        $scope.busy = false;

                        //show any notifications
                        if (angular.isArray(err.data.notifications)) {
                            for (var i = 0; i < err.data.notifications.length; i++) {
                                notificationsService.showNotification(err.data.notifications[i]);
                            }
                        }
                    });
            };

            $scope.dialogTreeEventHandler.bind("treeNodeSelect", nodeSelectHandler);
            $scope.dialogTreeEventHandler.bind("treeNodeExpanded", nodeExpandedHandler);

            $scope.$on('$destroy',
                function() {
                    $scope.dialogTreeEventHandler.unbind("treeNodeSelect", nodeSelectHandler);
                    $scope.dialogTreeEventHandler.unbind("treeNodeExpanded", nodeExpandedHandler);
                });
        }
);