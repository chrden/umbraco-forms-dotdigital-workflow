angular.module("umbraco.resources").factory("fieldmappingResource",
    function ($q, $http, umbRequestHelper) {
        // the factory object returned
        return {
            getAddressBooks: function () {
                return umbRequestHelper.resourcePromise(
                    $http.get("backoffice/CD/DotdigitalApi/GetAddressBooks"), "Failed to retrieve Dotdigital address books");
            },
            getDataFields: function () {
                return umbRequestHelper.resourcePromise(
                    $http.get("backoffice/CD/DotdigitalApi/GetDataFields"), "Failed to retrieve Dotdigital data fields");
            }
        }
    });