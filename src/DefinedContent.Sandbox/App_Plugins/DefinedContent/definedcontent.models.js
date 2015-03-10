var __extends = this.__extends || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var DefinedContent;
(function (DefinedContent) {
    var DefinedContentApiModel = (function () {
        function DefinedContentApiModel() {
            this.Key = "";
            this.ParentKey = "";
            this.ResolveType = "";
            this.ResolveValue = "";
            this.CreateConfig = new CreateConfiguration();
        }
        return DefinedContentApiModel;
    })();
    DefinedContent.DefinedContentApiModel = DefinedContentApiModel;
    var CreateConfiguration = (function () {
        function CreateConfiguration() {
            this.Enabled = false;
            this.Name = "";
            this.ContentTypeAlias = "";
            this.PropertyMapping = new Array();
        }
        return CreateConfiguration;
    })();
    DefinedContent.CreateConfiguration = CreateConfiguration;
    var PropertyMap = (function () {
        function PropertyMap() {
            this.Alias = "";
            this.Value = "";
            this.IsKey = false;
        }
        return PropertyMap;
    })();
    DefinedContent.PropertyMap = PropertyMap;
    var DefinedContentViewModel = (function (_super) {
        __extends(DefinedContentViewModel, _super);
        function DefinedContentViewModel() {
            _super.apply(this, arguments);
            this.XPathResolver = "";
            this.ContentIdResolver = "";
            this.KeyResolver = "";
        }
        return DefinedContentViewModel;
    })(DefinedContentApiModel);
    DefinedContent.DefinedContentViewModel = DefinedContentViewModel;
    var TypeConverter = (function () {
        function TypeConverter() {
        }
        TypeConverter.ViewModelToApiModel = function (viewModel) {
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
        };
        TypeConverter.ApiModelToViewModel = function (apiModel) {
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
        };
        return TypeConverter;
    })();
    DefinedContent.TypeConverter = TypeConverter;
    var WebApi = (function () {
        function WebApi(http) {
            this.$http = http;
        }
        WebApi.prototype.GetByKey = function (key) {
            return this.$http.get("/umbraco/api/DefinedContentEditorApi/Get?key=" + key);
        };
        WebApi.prototype.Save = function (model) {
            return this.$http.post("/umbraco/api/DefinedContentEditorApi/Save", angular.toJson(model));
        };
        WebApi.prototype.DeleteByKey = function (key) {
            return this.$http.delete("/umbraco/api/DefinedContentEditorApi/Delete?id=" + key);
        };
        return WebApi;
    })();
    DefinedContent.WebApi = WebApi;
})(DefinedContent || (DefinedContent = {}));
//# sourceMappingURL=definedcontent.models.js.map