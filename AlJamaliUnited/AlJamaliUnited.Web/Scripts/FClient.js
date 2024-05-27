if (Focus8WAPI == undefined || Focus8WAPI == null) {
    var Focus8WAPI = {
        ENUMS: {
            MODULE_TYPE: {
                MASTER: 1,
                TRANSACTION: 2,
                UI: 3,
                GLOBAL: 4,
                MRP: 5,
                FixedAsset: 6,
                TransHome: 7
            },

            REQUEST_TYPE: {
                GET: 1,
                SET: 2,
                CONTINUE: 3,
                RESET_CACHE: 4,
                DELETE_ROW: 5
            },

            REQUEST_TYPE_UI: {
                SET_POPUP_COORDINATE: 1,
                OPEN_POPUP: 2,
                CLOSE_POPUP: 3,
                GOTOHOMEPAGE: 4,
                OPEN_INVOICE_DESIGNER: 5,
                AWAKE_SESSION: 6,
                LOGOUT: 7,
                MANDATORY_FIELDS_ENTRYSCREEN: 8
            }
        },

        getFieldValue: function (sCallbackFn, Field, iModuleType, isFieldId, iRequestId, bStruct) {
            console.log("Method call - Focus8WAPI.getFieldValue");
            var obj = null;

            try {
                obj = {
                    moduleType: iModuleType,
                    rowIndex: 0,
                    isFieldId: isFieldId,
                    requestType: Focus8WAPI.ENUMS.REQUEST_TYPE.GET,
                    objData: { fieldid: Field },
                    iRequestId: iRequestId,
                    sCallbackFn: sCallbackFn,
                    bStruct: bStruct
                };

                if (Focus8WAPI.PRIVATE.isValidInput(obj, false) == true) {
                    Focus8WAPI.PRIVATE.postMessage(obj);
                }
            }
            catch (err) {
                alert("Exception: Focus8WAPI.getFieldValue " + err.message);
            }
        },

        setFieldValue: function (sCallbackFn, Field, Value, iModuleType, isFieldId, iRequestId, bStruct) {
            console.log("Method call - Focus8WAPI.setFieldValue");
            var obj = null;

            try {
                obj = {
                    moduleType: iModuleType,
                    rowIndex: 0,
                    isFieldId: isFieldId,
                    requestType: Focus8WAPI.ENUMS.REQUEST_TYPE.SET,
                    objData: { fieldid: Field, value: Value },
                    iRequestId: iRequestId,
                    sCallbackFn: sCallbackFn,
                    bStruct: bStruct
                };

                if (Focus8WAPI.PRIVATE.isValidInput(obj, false) == true) {
                    Focus8WAPI.PRIVATE.postMessage(obj);
                }
            }
            catch (err) {
                alert("Exception: Focus8WAPI.setFieldValue " + err.message);
            }
        },

        getBodyFieldValue: function (sCallbackFn, Field, iModuleType, isFieldId, iRowIndex, iRequestId, bStruct) {
            console.log("Method call - Focus8WAPI.getBodyFieldValue");
            var obj = null;

            try {
                obj = {
                    moduleType: iModuleType,
                    rowIndex: iRowIndex,
                    isFieldId: isFieldId,
                    requestType: Focus8WAPI.ENUMS.REQUEST_TYPE.GET,
                    objData: { fieldid: Field },
                    iRequestId: iRequestId,
                    sCallbackFn: sCallbackFn,
                    bStruct: bStruct
                };

                if (Focus8WAPI.PRIVATE.isValidInput(obj, true) == true) {
                    Focus8WAPI.PRIVATE.postMessage(obj);
                }
            }
            catch (err) {
                alert("Exception: Focus8WAPI.getBodyFieldValue " + err.message);
            }
        },

        setBodyFieldValue: function (sCallbackFn, Field, Value, iModuleType, isFieldId, iRowIndex, iRequestId, bStruct) {
            console.log("Method call - Focus8WAPI.setBodyFieldValue");
            var obj = null;

            try {
                obj = {
                    moduleType: iModuleType,
                    rowIndex: iRowIndex,
                    isFieldId: isFieldId,
                    requestType: Focus8WAPI.ENUMS.REQUEST_TYPE.SET,
                    objData: { fieldid: Field, value: Value },
                    iRequestId: iRequestId,
                    sCallbackFn: sCallbackFn,
                    bStruct: bStruct
                };

                if (Focus8WAPI.PRIVATE.isValidInput(obj, true) == true) {
                    Focus8WAPI.PRIVATE.postMessage(obj);
                }
            }
            catch (err) {
                alert("Exception: Focus8WAPI.setBodyFieldValue " + err.message);
            }
        },

        continueModule: function (iModuleType, result) {
            console.log("Method call - Focus8WAPI.continueModule");
            var obj = null;

            try {
                obj = {};
                obj.moduleType = iModuleType;
                obj.requestType = Focus8WAPI.ENUMS.REQUEST_TYPE.CONTINUE;
                obj.result = result;

                Focus8WAPI.PRIVATE.postMessage(obj);
            }
            catch (err) {
                alert("Exception: Focus8WAPI.continueModule " + err.message);
            }
        },

        deleteRows: function (iModuleType, iRowIndex, iCount) {
            console.log("Method call - Focus8WAPI.deleteRows");
            let obj = null;

            try {
                obj = {
                    moduleType: iModuleType,
                    requestType: Focus8WAPI.ENUMS.REQUEST_TYPE.DELETE_ROW,
                    rowIndex: iRowIndex,
                    rowCount: iCount
                };

                Focus8WAPI.PRIVATE.postMessage(obj);
            }
            catch (err) {
                alert("Exception: Focus8WAPI.deleteRows " + err.message);
            }
        },

        openPopup: function (url, sCallback) {
            console.log("Method call - Focus8WAPI.openPopup");
            var obj = null;

            try {
                if (Focus8WAPI.PRIVATE.isNullOrEmpty(url, true) == true) {
                    return (false);
                }

                obj = {};
                obj.URL = url;
                obj.moduleType = Focus8WAPI.ENUMS.MODULE_TYPE.UI;
                obj.requestType = Focus8WAPI.ENUMS.REQUEST_TYPE_UI.OPEN_POPUP;

                Focus8WAPI.PRIVATE.postMessage(obj);
            }
            catch (err) {
                alert("Exception: Focus8WAPI.openPopup " + err.message);
            }

            return (true);
        },

        closePopup: function () {
            console.log("Method call - Focus8WAPI.closePopup");
            var obj = null;

            try {
                obj = {};
                obj.moduleType = Focus8WAPI.ENUMS.MODULE_TYPE.UI;
                obj.requestType = Focus8WAPI.ENUMS.REQUEST_TYPE_UI.CLOSE_POPUP;

                Focus8WAPI.PRIVATE.postMessage(obj);
            }
            catch (err) {
                alert("Exception: Focus8WAPI.closePopup " + err.message);
            }
        },

        gotoHomePage: function () {
            console.log("Method call - Focus8WAPI.gotoHomePage");
            var obj = null;

            try {
                obj = {};
                obj.moduleType = Focus8WAPI.ENUMS.MODULE_TYPE.UI;
                obj.requestType = Focus8WAPI.ENUMS.REQUEST_TYPE_UI.GOTOHOMEPAGE;

                Focus8WAPI.PRIVATE.postMessage(obj);
            }
            catch (err) {
                alert("Exception: Focus8WAPI.gotoHomePage " + err.message);
            }
        },

        logout: function () {
            console.log("Method call - Focus8WAPI.logout");
            var obj = null;

            try {
                obj = {};
                obj.moduleType = Focus8WAPI.ENUMS.MODULE_TYPE.UI;
                obj.requestType = Focus8WAPI.ENUMS.REQUEST_TYPE_UI.LOGOUT;

                Focus8WAPI.PRIVATE.postMessage(obj);
            }
            catch (err) {
                alert("Exception: Focus8WAPI.logout " + err.message);
            }
        },

        awakeSession: function () {
            console.log("Method call - Focus8WAPI.awakeSession");
            var obj = null;

            try {
                obj = {};
                obj.moduleType = Focus8WAPI.ENUMS.MODULE_TYPE.UI;
                obj.requestType = Focus8WAPI.ENUMS.REQUEST_TYPE_UI.AWAKE_SESSION;

                Focus8WAPI.PRIVATE.postMessage(obj);
            }
            catch (err) {
                alert("Exception: Focus8WAPI.awakeSession " + err.message);
            }
        },

        getMandatoryFields: function (sCallback, iMasterTypeId) {
            console.log("Method call - Focus8WAPI.getMandatoryFields");
            var obj = null;

            try {
                obj = {};
                obj.moduleType = Focus8WAPI.ENUMS.MODULE_TYPE.MASTER;
                obj.requestType = Focus8WAPI.ENUMS.REQUEST_TYPE_UI.MANDATORY_FIELDS_ENTRYSCREEN;
                obj.sCallbackFn = sCallback;
                obj.objData = iMasterTypeId;
                Focus8WAPI.PRIVATE.postMessage(obj);
            }
            catch (err) {
                alert("Exception: Focus8WAPI.getMandatoryFields " + err.message);
            }
        },

        resetTransactionCache: function (iVoucherType) {
            console.log("Method call - Focus8WAPI.resetTransactionCache");
            var obj = null;

            try {
                obj = {};
                obj.moduleType = Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION;
                obj.requestType = Focus8WAPI.ENUMS.REQUEST_TYPE.RESET_CACHE;
                obj.iVoucherType = iVoucherType;

                Focus8WAPI.PRIVATE.postMessage(obj);
            }
            catch (err) {
                alert("Exception: Focus8WAPI.awakeSession " + err.message);
            }
        },

        setPopupCoordinates: function (sLeft, sTop, sWidth, sHeight) {
            console.log("Method call - Focus8WAPI.setPopupCoordinates");
            var obj = null;

            try {
                obj = {};
                obj.moduleType = Focus8WAPI.ENUMS.MODULE_TYPE.UI;
                obj.requestType = Focus8WAPI.ENUMS.REQUEST_TYPE_UI.SET_POPUP_COORDINATE;
                obj.Left = sLeft;
                obj.Top = sTop;
                obj.Width = sWidth;
                obj.Height = sHeight;
                Focus8WAPI.PRIVATE.postMessage(obj);
            }
            catch (err) {
                alert("Exception: Focus8WAPI.openPopup " + err.message);
            }

            return (true);
        },

        getGlobalValue: function (sCallbackFn, sVariable, iRequestId) {
            debugger
            console.log("Method call - Focus8WAPI.getGlobalValue");
            var obj = null;

            try {
                obj = {};
                obj.moduleType = Focus8WAPI.ENUMS.MODULE_TYPE.GLOBAL;
                obj.requestType = Focus8WAPI.ENUMS.REQUEST_TYPE.GET;
                obj.Variable = sVariable;
                obj.iRequestId = iRequestId;
                obj.sCallbackFn = sCallbackFn;

                Focus8WAPI.PRIVATE.postMessage(obj);
            }
            catch (err) {
                alert("Exception: Focus8WAPI.getGlobalValue " + err.message);
            }
        },

        openInvoiceDesigner: function (sCallbackFn, LayoutId, iVouchertype, iHeaderId, eModuleType, HeaderGroup, iSubReportId, bSaveHTMLSource, iRequestId) {
            console.log("Method call - Focus8WAPI.openInvoiceDesigner");
            var obj = null;

            try {
                obj = {};
                obj.moduleType = Focus8WAPI.ENUMS.MODULE_TYPE.UI;
                obj.requestType = Focus8WAPI.ENUMS.REQUEST_TYPE_UI.OPEN_INVOICE_DESIGNER;
                obj.LayoutId = LayoutId;
                obj.iVouchertype = iVouchertype;
                obj.iHeaderId = iHeaderId;
                obj.ModuleType = eModuleType;
                obj.HeaderGroup = HeaderGroup;
                obj.iSubReportId = iSubReportId;
                obj.bSaveHTMLSource = bSaveHTMLSource;
                obj.sCallbackFn = sCallbackFn;
                obj.iRequestId = iRequestId;
                Focus8WAPI.PRIVATE.postMessage(obj);
                return obj;
            }
            catch (err) {
                alert("Exception: Focus8WAPI.openPopup " + err.message);
            }
        },

        PRIVATE: {
            isValidInput: function (obj, bBodyField) {
                try {
                    if (Focus8WAPI.PRIVATE.isValidObject(obj.moduleType) == false || obj.moduleType.toString() == "") {
                        alert("Validation Exception: Please pass Module Type parameter");

                        return (false);
                    }

                    if (Focus8WAPI.PRIVATE.isValidObject(obj.isFieldId) == false || obj.isFieldId.toString() == "") {
                        alert("Validation Exception: Please pass isFieldId parameter");

                        return (false);
                    }

                    if (Focus8WAPI.PRIVATE.isValidObject(obj.objData.fieldid) == false) {
                        alert("Validation Exception: Please pass Field parameter");

                        return (false);
                    }
                    else {
                        if (Array.isArray(obj.objData.fieldid) == true) {
                            if (obj.objData.fieldid.length == 0) {
                                alert("Validation Exception: Please pass Field parameter");

                                return (false);
                            }
                        }
                    }


                    if (bBodyField == true) {
                        if (Focus8WAPI.PRIVATE.isValidObject(obj.rowIndex) == false) {
                            alert("Validation Exception: Row Index should be number type");

                            return (false);
                        }

                        if (Array.isArray(obj.rowIndex) == false) {
                            if (isNaN(obj.rowIndex)) {
                                alert("Validation Exception: Row Index should be number type");

                                return (false);
                            }

                            if (obj.rowIndex == 0) {
                                alert("Validation Exception: Row Index should be greater than 0 for Body Fields");

                                return (false);
                            }
                        }
                    }
                }
                catch (err) {
                    alert("Exception: {Focus8WAPI.PRIVATE.isValidInput} " + err.message);
                }

                return (true);
            },

            postMessage: function (obj) {
                try {
                    obj.FromClient = true;
                    window.parent.postMessage(obj, "*");
                }
                catch (err) {
                    alert("Exception: Focus8WAPI.PRIVATE.postMessage " + err.message);
                }
            },

            onReceiveMessage: function (evt) {
                debugger
                var objReturnData = null;
                var obj = null;

                try {
                    Focus8WAPI.PRIVATE.stopKeyProcess(evt);
                    objReturnData = evt.data;

                    // Client                
                    if (Focus8WAPI.PRIVATE.isValidObject(objReturnData.FromClient) == true) {
                        return;
                    }

                    console.log('Focus8WAPI::Received Response: ', JSON.stringify(objReturnData));

                    if (Focus8WAPI.PRIVATE.isNullOrEmpty(objReturnData.sCallbackFn, true) == false) {
                        obj = {};
                        obj.returnCode = objReturnData.response.lValue;
                        obj.message = objReturnData.response.sValue;
                        obj.data = objReturnData.response.data;
                        obj.fieldId = objReturnData.fieldId;
                        obj.requestType = objReturnData.requestType;
                        obj.moduleType = objReturnData.moduleType;
                        obj.iRequestId = objReturnData.iRequestId;

                        if (Focus8WAPI.PRIVATE.isValidObject(objReturnData.RowsInfo) == true) {
                            obj.RowsInfo = objReturnData.RowsInfo;
                        }

                        eval(objReturnData.sCallbackFn)(obj);
                    }
                }
                catch (err) {
                    alert("Exception: Focus8WAPI.PRIVATE.onReceiveMessage " + err.message);
                }
            },

            isValidObject: function (obj) {
                try {
                    if (typeof obj == "undefined" || obj == null) {
                        return (false);
                    }

                    return (true);
                }
                catch (err) {
                    alert("Exception: {Focus8WAPI.PRIVATE.isValidObject} " + err.message);
                }

                return (false);
            },

            isNullOrEmpty: function (sValue, bTrim) {
                var bResult = false;

                try {
                    if (Focus8WAPI.PRIVATE.isValidObject(sValue) == false || (typeof sValue).toLowerCase() != "string" || sValue.length <= 0) {
                        return (true);
                    }

                    if (Focus8WAPI.PRIVATE.isValidObject(bTrim) == true && bTrim == true) {
                        if (sValue.trim().length == 0) {
                            return (true);
                        }
                    }
                }
                catch (err) {
                    alert("Exception: {Focus8WAPI.PRIVATE.isNullOrEmpty} " + err.message);
                    bResult = true;
                }

                return (bResult);
            },

            stopKeyProcess: function (evt) {
                try {
                    if (Focus8WAPI.PRIVATE.isValidObject(evt) == false) {
                        return;
                    }

                    if (evt.preventDefault) {
                        evt.preventDefault();
                    }
                    else {
                        evt.returnValue = false;
                    }

                    if (evt.bubbles == true) {
                        evt.stopPropagation();
                    }
                }
                catch (err) {
                    alert("Exception: {Focus8WAPI.PRIVATE.stopKeyProcess} " + err.message);
                }
            }
        }

    }
    window.addEventListener('message', Focus8WAPI.PRIVATE.onReceiveMessage);
}
