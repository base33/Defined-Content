angular.module("umbraco.resources")
    .factory("definedContentWebApi", function ($http: ng.IHttpService) {
        return new DefinedContent.WebApi($http);
    }); 