"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.CustomUserElement = void 0;
var CustomUserElement = /** @class */ (function () {
    function CustomUserElement() {
        this.id = 0;
        this.name = '';
        this.value = '';
        this.templateValue = null;
        this.elementSotringOrder = null;
        this.userPatternId = null;
    }
    return CustomUserElement;
}());
exports.CustomUserElement = CustomUserElement;
var SortingOrder;
(function (SortingOrder) {
    SortingOrder[SortingOrder["BeforeText"] = 0] = "BeforeText";
    SortingOrder[SortingOrder["AfterText"] = 1] = "AfterText";
})(SortingOrder || (SortingOrder = {}));
//# sourceMappingURL=CustomUserElement.js.map