﻿var app = angular.module('taskApp', []);

app.controller('taskController', function ($scope, $http) {
    $scope.tasks = [];

    $scope.Init = function (tasks) {
        $scope.tasks = tasks.Tasks;
    };

    $scope.AddTask = function () {
        $scope.AddTask(null, null);
    }

    $scope.AddTask = function (dueDate, dueTime) {
        var taskId = GenerateGuid();
        var type = "Pending";
        if (dueDate != null) {
            type = "Event";
        }
        var task = { Id: taskId, Name: $scope.TaskName, Type: type }
        $scope.tasks.push(task);

        $http({
            url: "Home/Add",
            method: "POST",
            params: { id: taskId, name: $scope.TaskName, date: dueDate, time: dueTime }
        });
        $scope.TaskName = "";

    };

    $scope.AddEvent = function () {
        $scope.ShowAdvanced = !$scope.ShowAdvanced;
    }

    $scope.AddRecurring = function () {
        $scope.ShowRecurring = !$scope.ShowRecurring;
    }

    $scope.AddRecurringTask = function () {
        var taskId = GenerateGuid();

        if ($scope.Period == null) {
            $scope.AddTask($scope.Date, $scope.Time);
        }
        else {
            var task = { Id: taskId, Name: $scope.TaskName, Type: type };
            $scope.tasks.push(task);
            $http.post("Home/AddRecurringTask", { id: taskId, name: $scope.TaskName, period: $scope.Period, frequency: $scope.Frequency });
            $scope.TaskName = "";
            $scope.ShowRecurring = !$scope.ShowRecurring;
            $scope.Period = "";
            $scope.Frequency = "";
        }
        $scope.TaskName = "";
        $scope.ShowAdvanced = !$scope.ShowAdvanced;
    }

    $scope.RemoveTask = function (taskId, index) {
        $scope.tasks.splice(index, 1);
        $http.post("Home/Remove", { id: taskId.Id });
    }

    $scope.GenerateSchedule = function () {
        $http.post("Home/AddSchedule", { from: $scope.From, to: $scope.To });
        setTimeout(function () {
            window.location.reload();
        }, 1000);
    }

    $scope.EditTask = function (taskId, index) {
        alert("Edit not implemented");
    }

    function S4() {
        return (((1 + Math.random()) * 0x10000) | 0).toString(16).substring(1);
    }

    function GenerateGuid() {
        return (S4() + S4() + "-" + S4() + "-4" + S4().substr(0, 3) + "-" + S4() + "-" + S4() + S4() + S4()).toLowerCase();
    }
});