﻿module DefinedContent {

    export class DefinedContentApiModel {
        public Key: string = "";
        public ParentKey: string = "";
        public ResolveType: string = "";
        public ResolveValue: string = "";

        public CreateConfig: CreateConfiguration = new CreateConfiguration();
    }

    export class CreateConfiguration {
        public Enabled: boolean = false;
        public Name: string = "";
        public ContentTypeAlias: string = "";
        public PropertyMapping: Array<PropertyMap> = new Array<PropertyMap>();
    }

    export class PropertyMap {
        public Alias: string = "";
        public Value: string = "";
        public IsKey: boolean = false;
    }

    export class DefinedContentViewModel extends DefinedContentApiModel {
        public XPathResolver: string = "";
        public ContentIdResolver: string = "";
        public KeyResolver: string = "";

        
    }

    export class TypeConverter {
        public static ViewModelToApiModel(viewModel: DefinedContentViewModel): DefinedContentApiModel {
            var model = new DefinedContentApiModel();
            model.Key = viewModel.Key;
            model.ParentKey = viewModel.ParentKey;
            model.CreateConfig = viewModel.CreateConfig;
            model.ResolveType = viewModel.ResolveType;
            switch (model.ResolveType) {
                case "xpath":
                    model.ResolveValue = viewModel.XPathResolver;
                    break;
                case "contentId":
                    model.ResolveValue = viewModel.ContentIdResolver;
                    break;
                case "key":
                    model.ResolveValue = viewModel.KeyResolver;
                    break;
            }
            return model;
        }

        public static ApiModelToViewModel(apiModel: DefinedContentApiModel): DefinedContentViewModel {
            var model = new DefinedContentViewModel();
            model.Key = apiModel.Key;
            model.ParentKey = apiModel.ParentKey;
            model.CreateConfig = apiModel.CreateConfig;
            model.ResolveType = apiModel.ResolveType;
            switch (model.ResolveType) {
                case "xpath":
                    model.XPathResolver = apiModel.ResolveValue;
                    break;
                case "contentId":
                    model.ContentIdResolver = apiModel.ResolveValue;
                    break;
                case "key":
                    model.KeyResolver = apiModel.ResolveValue;
                    break;
            }
            return model;
        }
    }

    export class WebApi {
        public $http: ng.IHttpService;

        public constructor(http: ng.IHttpService) {
            this.$http = http;
        }

        public GetByKey(key: string): ng.IHttpPromise<DefinedContent.DefinedContentApiModel> {
            return this.$http.get<DefinedContent.DefinedContentApiModel>("/umbraco/api/DefinedContentEditorApi/Get?key=" + key);
        }

        public Save(model: DefinedContent.DefinedContentApiModel): ng.IHttpPromise<boolean> {
            return this.$http.post("/umbraco/api/DefinedContentEditorApi/Save", angular.toJson(model));
        }

        public DeleteByKey(key: string): ng.IHttpPromise<boolean> {
            return this.$http.delete("/umbraco/api/DefinedContentEditorApi/Delete?id=" + key);
        }
    }
}