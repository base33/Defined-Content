angular.module("umbraco.resources")
    .factory("definedContentResource", function ($http) {
        return {
            GetByKey: function(key) {
                return $http.get("backoffice/Example/PersonApi/GetById?id=" + id);  
            },
            Save: function(person) {
                return $http.post("backoffice/Example/PersonApi/PostSave", angular.toJson(person));
            },
            DeleteByKey: function(id) {
                return $http.delete("backoffice/Example/PersonApi/DeleteById?id=" + id);
            }
        };
    });

export class Something {

}