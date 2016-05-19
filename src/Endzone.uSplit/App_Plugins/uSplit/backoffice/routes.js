//register an MVC route

app.config(function ($routeProvider) {

    /** This checks if the user is authenticated for a route and what the isRequired is set to.
        Depending on whether isRequired = true, it first check if the user is authenticated and will resolve successfully
        otherwise the route will fail and the $routeChangeError event will execute, in that handler we will redirect to the rejected
        path that is resolved from this method and prevent default (prevent the route from executing) */
    var canRoute = function (isRequired) {

        return {
            /** Checks that the user is authenticated, then ensures that are requires assets are loaded */
            isAuthenticatedAndReady: function ($q, userService, $route, assetsService, appState) {
                var deferred = $q.defer();

                //don't need to check if we've redirected to login and we've already checked auth
                if (!$route.current.params.section
                    && ($route.current.params.check === false || $route.current.params.check === "false")) {
                    deferred.resolve(true);
                    return deferred.promise;
                }

                userService.isAuthenticated()
                    .then(function () {

                        assetsService._loadInitAssets().then(function () {

                            //This could be the first time has loaded after the user has logged in, in this case
                            // we need to broadcast the authenticated event - this will be handled by the startup (init)
                            // handler to set/broadcast the ready state
                            var broadcast = appState.getGlobalState("isReady") !== true;

                            userService.getCurrentUser({ broadcastEvent: broadcast }).then(function (user) {
                                //is auth, check if we allow or reject
                                if (isRequired) {
                                    // U4-5430, Benjamin Howarth
                                    // We need to change the current route params if the user only has access to a single section
                                    // To do this we need to grab the current user's allowed sections, then reject the promise with the correct path.
                                    if (user.allowedSections.indexOf($route.current.params.section) > -1) {
                                        //this will resolve successfully so the route will continue
                                        deferred.resolve(true);
                                    } else {
                                        deferred.reject({ path: "/" + user.allowedSections[0] });
                                    }
                                }
                                else {
                                    deferred.reject({ path: "/" });
                                }
                            });

                        });

                    }, function () {
                        //not auth, check if we allow or reject
                        if (isRequired) {
                            //the check=false is checked above so that we don't have to make another http call to check
                            //if they are logged in since we already know they are not.
                            deferred.reject({ path: "/login/false" });
                        }
                        else {
                            //this will resolve successfully so the route will continue
                            deferred.resolve(true);
                        }
                    });
                return deferred.promise;
            }
        };
    }

    $routeProvider
        .when('/:section/:tree/:method/:id/mvc', {
            //This allows us to dynamically change the template for this route since you cannot inject services into the templateUrl method.
            //template: "<div ng-include='templateUrl'></div>",
            template: "<iframe src='{{templateUrl}}' style='width: 100%; height: 100%; background:none;'></iframe>",
            //This controller will execute for this route, then we replace the template dynamnically based on the current tree.
            controller: function ($scope, $route, $routeParams, treeService) {

                if (!$routeParams.tree || !$routeParams.method) {
                    $scope.templateUrl = "views/common/dashboard.html";
                }

                var packageTreeFolder = treeService.getTreePackageFolder($routeParams.tree);
                $scope.templateUrl = Umbraco.Sys.ServerVariables.umbracoSettings.umbracoPath + "/backoffice/" + packageTreeFolder + "/" + $routeParams.method + "/" + $routeParams.id;
            },
            resolve: canRoute(true)
        })

;
});
