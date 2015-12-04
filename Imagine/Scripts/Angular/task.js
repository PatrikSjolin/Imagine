var app = angular.module('taskApp', []);

app.controller('taskController', function ($scope, $http) {
    $scope.tasks = [];

    $scope.Init = function (tasks)
    {
        $scope.tasks = tasks.Tasks;
    };

    $scope.AddTask = function ()
    {
        var taskId = GenerateGuid();
        var task = { Name: $scope.taskName, Id: taskId }
        $scope.tasks.push(task);

        $http({
            url: "Home/Add",
            method: "POST",
            params: { name: $scope.taskName, id: taskId }
        });
        $scope.taskName = "";

    };

    $scope.RemoveTask = function (taskId, index)
    {
        $scope.tasks.splice(index, 1);
        $http.post("Home/Remove", { id: taskId.Id });
    }

    function S4() {
        return (((1 + Math.random()) * 0x10000) | 0).toString(16).substring(1);
    }

    function GenerateGuid() {
        return (S4() + S4() + "-" + S4() + "-4" + S4().substr(0, 3) + "-" + S4() + "-" + S4() + S4() + S4()).toLowerCase();
    }
});