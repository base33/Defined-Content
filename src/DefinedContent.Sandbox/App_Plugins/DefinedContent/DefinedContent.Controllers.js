angular.module("umbraco").controller("DefinedContent.KeyCreateController", function ($scope, notificationsService, navigationService, $location, $routeParams, definedContentWebApi) {
    $scope.record = new DefinedContent.DefinedContentViewModel();
    var currentRecord = $scope.record;
    $scope.isRoot = $routeParams.id == -1;
    $scope.save = function (record) {
        var apiModel = DefinedContent.TypeConverter.ViewModelToApiModel(record);
        definedContentWebApi.ValidateModel(apiModel, true).success(function (errors) {
            if (errors.length == 0) {
                definedContentWebApi.Save(apiModel).success(function (result) {
                    notificationsService.success("Saved");
                    $location.path("/settings/definedContent/edit/" + record.Key);
                }).error(function (result) {
                    notificationsService.error("Unable to save.  See log");
                });
            }
            else {
                notificationsService.error(errors.join());
            }
        });
    };
    $scope.createEnabledStyle = function () {
        return $scope.record.CreateConfig.Enabled ? {} : { "background": "rgb(208, 208, 208)", "opacity": "0.6" };
    };
    $scope.addProperty = function () {
        var property = currentRecord.CreateConfig.PropertyMapping[currentRecord.CreateConfig.PropertyMapping.length - 1];
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
angular.module("umbraco").controller("DefinedContent.KeyEditController", function ($scope, notificationsService, navigationService, $location, $routeParams, definedContentWebApi) {
    $scope.loaded = false;
    $scope.record = null;
    var currentRecord = null;
    $scope.save = function (record) {
        var apiModel = DefinedContent.TypeConverter.ViewModelToApiModel(record);
        definedContentWebApi.ValidateModel(apiModel, false).success(function (errors) {
            if (errors.length == 0) {
                definedContentWebApi.Save(apiModel).success(function (result) {
                    notificationsService.success("Saved");
                    record.CreateConfig.PropertyMapping.push(new DefinedContent.PropertyMap()); //insert the additional back up
                }).error(function (result) {
                    notificationsService.error("Unable to save.  See log");
                });
            }
            else {
                notificationsService.error(errors.join());
            }
        });
    };
    $scope.addProperty = function () {
        var property = currentRecord.CreateConfig.PropertyMapping[currentRecord.CreateConfig.PropertyMapping.length - 1];
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
        definedContentWebApi.GetByKey($routeParams.id).success(function (result) {
            $scope.record = DefinedContent.TypeConverter.ApiModelToViewModel(result);
            currentRecord = $scope.record;
            currentRecord.CreateConfig.PropertyMapping.push(new DefinedContent.PropertyMap());
            $scope.loaded = true;
        });
    }
    init();
});
angular.module("umbraco").controller("DefinedContent.KeyDeleteController", function ($scope, navigationService, $routeParams, definedContentWebApi) {
    $scope.id = $scope.$parent.currentAction.metaData.id;
    $scope.delete = function (id) {
        definedContentWebApi.DeleteByKey(id);
        navigationService.hideNavigation();
    };
    $scope.cancelDelete = function () {
        navigationService.hideNavigation();
    };
});
angular.module("umbraco").controller("DefinedContent.FullRefreshController", function ($scope, navigationService, notificationsService, $routeParams, definedContentWebApi) {
    $scope.doFullRefresh = function (id) {
        definedContentWebApi.FullRefresh();
        navigationService.hideNavigation();
        notificationsService.success("Full refresh triggered");
    };
    $scope.cancel = function () {
        navigationService.hideNavigation();
    };
});
//# sourceMappingURL=DefinedContent.Controllers.js.map