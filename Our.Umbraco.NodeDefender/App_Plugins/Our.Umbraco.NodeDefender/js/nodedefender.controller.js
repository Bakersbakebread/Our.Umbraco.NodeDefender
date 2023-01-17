angular
    .module("umbraco")
    .controller("NodeDefender", function ($scope, NodeDefenderResource) {

        var vm = this;

        NodeDefenderResource.fetchCurrentSettings()
            .then(function (response) {
                vm.settings = response.data;
                vm.denyRename = vm.settings.DenyOptionsDtos.filter(function (item) {
                    return item.Type === "Rename";
                })[0];
                vm.denyDelete = vm.settings.DenyOptionsDtos.filter(function (item) {
                    return item.Type === "Delete";
                })[0];
                vm.denyDuplicate = vm.settings.DenyOptionsDtos.filter(function (item) {
                    return item.Type === "Duplicate";
                })[0];
            })
            .catch(function (response) {
                console.warn(response);
            });
        

    });