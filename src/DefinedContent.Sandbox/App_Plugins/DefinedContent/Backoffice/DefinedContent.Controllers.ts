/// <reference path="../../../scripts/typings/angularjs/angular.d.ts"/>

angular.module("umbraco").controller("DefinedContent.KeyCreateController",
    function ($scope, navigationService, $routeParams) {
        $scope.record = {
            name: "",
            type: "id",
            contentId: "",
            xpath: "",
            key: "",
            create: {
                enable: false,
                name: "",
                contentTypeAlias: "",
                properties: []
            }
        };

        $scope.addProperty = function () {
            var entry = $scope.createPropertyEntry;
            var property = {
                alias: entry.propertyAlias,
                value: entry.propertyValue,
                isKey: entry.valueIsKey
            };
            $scope.record.create.properties.push(property);
        };

        $scope.deleteProperty = function (index) {
            $scope.remove.create.properties.splice(index, 1);
        };
    });

angular.module("umbraco").controller("DefinedContent.KeyEditController",
    function ($scope, navigationService, $routeParams) {
        
    });

angular.module("umbraco").controller("DefinedContent.KeyDeleteController",
    function ($scope, navigationService, $routeParams) {
        $scope.id = $scope.$parent.currentAction.metaData.id;

        $scope.delete = function (id) {
            navigationService.hideNavigation();
        };

        $scope.cancelDelete = function () {
            navigationService.hideNavigation();
        };
    });