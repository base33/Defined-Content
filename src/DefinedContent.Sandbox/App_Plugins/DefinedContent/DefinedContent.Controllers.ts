angular.module("umbraco").controller("DefinedContent.KeyCreateController",
    function ($scope, notificationsService, navigationService, $location: ng.ILocationService, $routeParams, definedContentWebApi: DefinedContent.WebApi) {
        $scope.record = new DefinedContent.DefinedContentViewModel();
        var currentRecord = <DefinedContent.DefinedContentViewModel>$scope.record;
        $scope.isRoot = $routeParams.id == -1;
        $scope.save = function (record: DefinedContent.DefinedContentViewModel) {
            var apiModel = DefinedContent.TypeConverter.ViewModelToApiModel(record);

            //switch (record.ResolveType) {
            //    case "xpath":
            //        apiModel.ResolveValue = record.XPathResolver;
            //        break;
            //    case "key":
            //        apiModel.ResolveValue = record.KeyResolver;
            //        break;
            //    case "contentId":
            //        apiModel.ResolveValue = record.ContentIdResolver;
            //        break;
            //}

            //if ($scope.isRoot) {
            //    switch (record.ParentResolveType) {
            //        case "xpath":
            //            apiModel.ParentKey = record.ParentXPathResolver;
            //            break;
            //        case "key":
            //            apiModel.ParentKey = record.ParentKeyResolver;
            //            break;
            //        case "contentId":
            //            apiModel.ParentKey = record.ParentContentIdResolver;
            //            break;
            //    }
            //}

            definedContentWebApi.Save(apiModel)
                .success((result) => {
                notificationsService.success("Saved");
                $location.path("/settings/definedContent/edit/" + record.Key);
            })
                .error((result) => {
                notificationsService.error("Unable to save.  See log");
            });
        }

        $scope.createEnabledStyle = function () {
            return $scope.record.CreateConfig.Enabled ? {} : { "background": "rgb(208, 208, 208)", "opacity" : "0.6" };
        };

        $scope.addProperty = function () {
            var property = <DefinedContent.PropertyMap>currentRecord.CreateConfig.PropertyMapping[currentRecord.CreateConfig.PropertyMapping.length - 1];

            if (property.Alias == "" || property.Value == "") {
                notificationsService.error("Please enter the property alias and default value");
                return;
            }

            if (!/^[a-zA-Z0-9]*$/.test(property.Alias)) {
                notificationsService.error("Property aliases can only contain letters or numbers.  No symbols or spaces allowed");
                return;
            }

            var exists = false;

            for (var i = 0; i < currentRecord.CreateConfig.PropertyMapping.length; i++) {
                if (currentRecord.CreateConfig.PropertyMapping[i].Alias == property.Alias && currentRecord.CreateConfig.PropertyMapping[i] !== property)
                    exists = true;
            }

            if (exists) {
                notificationsService.error("Property already exists");
                return;
            }

            currentRecord.CreateConfig.PropertyMapping.push(new DefinedContent.PropertyMap());
        };

        $scope.deleteProperty = function (index) {
            currentRecord.CreateConfig.PropertyMapping.splice(index, 1);
        };

        function init() {
            //add a blank property for the user to fill
            currentRecord.CreateConfig.PropertyMapping.push(new DefinedContent.PropertyMap());
            currentRecord.ResolveType = "contentId";
            currentRecord.ParentResolveType = "key";
            if ($routeParams.id != "-1") {
                currentRecord.ParentKey = $routeParams.id;
            }
            currentRecord.DefinedContentParent = $routeParams.id;
        }

        init();
    });

angular.module("umbraco").controller("DefinedContent.KeyEditController",
    function ($scope, notificationsService, navigationService, $location: ng.ILocationService, $routeParams, definedContentWebApi: DefinedContent.WebApi) {
        $scope.loaded = false;
        $scope.record = null;
        var currentRecord = null

        $scope.save = function (record: DefinedContent.DefinedContentViewModel) {
            var apiModel = DefinedContent.TypeConverter.ViewModelToApiModel(record);
            //switch (record.ResolveType) {
            //    case "xpath":
            //        apiModel.ResolveValue = record.XPathResolver;
            //        break;
            //    case "key":
            //        apiModel.ResolveValue = record.KeyResolver;
            //        break;
            //    case "contentId":
            //        apiModel.ResolveValue = record.ContentIdResolver;
            //        break;
            //}
            definedContentWebApi.Save(apiModel)
                .success((result) => {
                notificationsService.success("Saved");
            })
                .error((result) => {
                notificationsService.error("Unable to save.  See log");
            });
        }

        $scope.addProperty = function () {
            var property = <DefinedContent.PropertyMap>currentRecord.CreateConfig.PropertyMapping[currentRecord.CreateConfig.PropertyMapping.length - 1];

            if (property.Alias == "" || property.Value == "") {
                notificationsService.error("Please enter the property alias and default value");
                return;
            }

            if (!/^[a-zA-Z0-9]*$/.test(property.Alias)) {
                notificationsService.error("Property aliases can only contain letters or numbers.  No symbols or spaces allowed");
                return;
            }

            var exists = false;

            for (var i = 0; i < currentRecord.CreateConfig.PropertyMapping.length; i++) {
                if (currentRecord.CreateConfig.PropertyMapping[i].Alias == property.Alias && currentRecord.CreateConfig.PropertyMapping[i] !== property)
                    exists = true;
            }

            if (exists) {
                notificationsService.error("Property already exists");
                return;
            }

            currentRecord.CreateConfig.PropertyMapping.push(new DefinedContent.PropertyMap());
        };

        $scope.deleteProperty = function (index) {
            currentRecord.CreateConfig.PropertyMapping.splice(index, 1);
        };

        $scope.createEnabledStyle = function () {
            return $scope.record.CreateConfig.Enabled ? {} : { "background": "rgb(208, 208, 208)", "opacity": "0.6" };
        };

        function init() {
            definedContentWebApi.GetByKey($routeParams.id)
                .success((result) => {
                    $scope.record = DefinedContent.TypeConverter.ApiModelToViewModel(result); 
                    currentRecord = $scope.record;
                    currentRecord.CreateConfig.PropertyMapping.push(new DefinedContent.PropertyMap());
                    $scope.loaded = true;
            });
        }

        init();
    });

angular.module("umbraco").controller("DefinedContent.KeyDeleteController",
    function ($scope, navigationService, $routeParams, definedContentWebApi: DefinedContent.WebApi) {
        $scope.id = $scope.$parent.currentAction.metaData.id;

        $scope.delete = function (id) {
            definedContentWebApi.DeleteByKey(id)
            navigationService.hideNavigation();
        };

        $scope.cancelDelete = function () {
            navigationService.hideNavigation();
        };
    });