angular.module("umbraco").controller("CD.DotdigitalFieldMapping",
    function ($scope, fieldmappingResource) {

        $scope.model.error = false;
        $scope.model.loading = true;
        $scope.model.fieldMappings = [];

        $scope.model.mapBtn = {
            default: {
                labelKey: "mapButton_defaultLabel",
                handler: function () {
                    $scope.addFieldMapping(false);
                }
            },
            secondary: [
                {
                    labelKey: "mapButton_secondaryLabel",
                    handler: function () {
                        $scope.addFieldMapping(true);
                    }
                }
            ]
        }

        function init() {
            fieldmappingResource.getAddressBooks().then(function (response) {
                if (response) {
                    $scope.model.addressBooks = response;
                }
                else {
                    $scope.model.error = true;
                }
                $scope.model.loading = false;
            }, function () {
                $scope.model.error = true;
                $scope.model.loading = false;
            });

            if (!$scope.model.error && $scope.setting.value) {
                var result = JSON.parse($scope.setting.value);

                $scope.model.addressBookId = result.addressBookId;
                $scope.getDataFields();

                if (result.fieldMappings) {
                    $scope.model.fieldMappings = result.fieldMappings;
                }
            }
        }

        $scope.getDataFields = function () {
            if ($scope.model.addressBookId) {
                save();

                fieldmappingResource.getDataFields($scope.model.addressBookId).then(function (response) {
                    $scope.model.dataFields = response;
                });
            }
            else {
                $scope.model.fieldMappings = [];
            }
        }

        $scope.addFieldMapping = function (isStatic) {
            save();

            $scope.model.fieldMappings.push({
                formFieldAlias: "",
                dataFieldId: "",
                staticValue: "",
                isStatic
            });
        };

        $scope.deleteFieldMapping = function (i) {
            $scope.model.fieldMappings.splice(i, 1);

            save();
        };

        $scope.saveMapping = function () {
            save();
        };

        function save() {
            if ($scope.model.addressBookId && $scope.model.fieldMappings.length > 0) {
                $scope.setting.value =
                    JSON.stringify({
                        addressBookId: $scope.model.addressBookId,
                        fieldMappings: $scope.model.fieldMappings
                    });
            }
            else {
                $scope.setting.value = null;
            }
        }

        init();
    }
);