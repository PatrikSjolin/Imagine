var app = angular.module('taskApp', []);

app.controller('taskController', function ($scope, $http) {
    $scope.tasks = [];

    $scope.Init = function (tasks) {
        $scope.tasks = tasks.Tasks;
        $scope.TimeBased = true;
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
            params: { id: taskId, name: $scope.TaskName, date: dueDate, time: dueTime, duration: $scope.Duration }
        });
        Clear();

    };

    $scope.TimeBased = function () {
        $scope.TimeBased = !$scope.TimeBased;
    }

    $scope.AddEvent = function () {
        $scope.ShowAdvanced = !$scope.ShowAdvanced;
    }

    $scope.AddRecurring = function () {
        $scope.ShowRecurring = !$scope.ShowRecurring;
    }

    $scope.AddRecurringTask = function () {
        var taskId = GenerateGuid();

        if (!$scope.ShowRecurring) {
            $scope.AddTask($scope.Date, $scope.Time);
        }
        else {
            var type = "Recurring";
            var task = { Id: taskId, Name: $scope.TaskName, Type: type };
            $scope.tasks.push(task);
            if ($scope.TimeBased) {

                $http.post("Home/AddRecurringTask", { id: taskId, name: $scope.TaskName, period: $scope.Period, frequency: null, date: $scope.Date, duration: $scope.Duration });
            }
            else {
                $http.post("Home/AddRecurringTask", { id: taskId, name: $scope.TaskName, period: $scope.Period, frequency: $scope.Frequency, date: null, duration: $scope.Duration });
            }
            Clear();
        }
    }

    function Clear() {
        $scope.TaskName = "";
        $scope.Period = "";
        $scope.Frequency = "";
        $scope.Duration = "";
        $scope.Date = "";
        $scope.Time = "";
        $scope.ShowAdvanced = false;
        $scope.ShowRecurring = false;
        $scope.TimeBased = false;
    }

    $scope.Lock = function (task) {
        $http.post("Home/Lock", { id: task.Id })
    }

    $scope.RemoveTask = function (taskId, index) {
        $scope.tasks.splice(index, 1);
        $http.post("Home/Remove", { id: taskId.Id });
    }

    $scope.GenerateSchedule = function () {
        $http.post("Home/AddSchedule", { from: $scope.From, to: $scope.To });
        setTimeout(function () {
            window.location.reload();
        }, 1500);
    }

    $scope.EditTask = function (taskId, index) {
        alert("Edit not implemented");
    }

    $scope.CompleteTask = function (task) {
        alert("Complete not implemented");
    }

    function S4() {
        return (((1 + Math.random()) * 0x10000) | 0).toString(16).substring(1);
    }

    function GenerateGuid() {
        return (S4() + S4() + "-" + S4() + "-4" + S4().substr(0, 3) + "-" + S4() + "-" + S4() + S4() + S4()).toLowerCase();
    }
});