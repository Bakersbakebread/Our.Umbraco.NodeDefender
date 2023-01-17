angular.module("umbraco.resources")
    .factory("NodeDefenderResource", function ($http, umbRequestHelper) {
    return {
        fetchCurrentSettings: function () {
            return $http.get('/umbraco/NodeDefender/NodeDefenderDashboard/GetNodeDefenderSettings');
        }
    };
});