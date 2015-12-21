var app = angular.module('taskApp', ['ui.bootstrap']);

app.controller('taskController', function ($scope, $http, $window, $uibModal) {

    $scope.Init = function (tasks, from, to, time) {
        $scope.Tasks = tasks.Tasks;
        $scope.ScheduledTasks = tasks.ScheduledTasks;
        $scope.TimeBased = true;
        $scope.Period = "Month";
        $scope.Generating = false;
        $scope.From = new Date(Date.parse(from));
        $scope.To = new Date(Date.parse(to));
        $scope.Date = new Date(Date.parse(from));
        $scope.Time = new Date(Date.parse(time));
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
        $scope.Tasks.push(task);

        $http({
            url: "Home/Add",
            method: "POST",
            params: { id: taskId, name: $scope.TaskName, date: dueDate, time: dueTime, duration: $scope.Duration }
        });
        Clear();

        var inputField = $window.document.getElementById("taskInputField");
        inputField.focus();
    };

    $scope.AddRecurringTask = function () {
        var taskId = GenerateGuid();

        if (!$scope.ShowRecurring) {
            $scope.AddTask($scope.Date, $scope.Time);
        }
        else {
            var type = "Recurring";
            var task = { Id: taskId, Name: $scope.TaskName, Type: type };
            $scope.Tasks.push(task);
            if ($scope.TimeBased) {
                $http.post("Home/AddRecurringTask", { id: taskId, name: $scope.TaskName, period: $scope.Period, frequency: null, date: $scope.Date, duration: $scope.Duration });
            }
            else {
                $http.post("Home/AddRecurringTask", { id: taskId, name: $scope.TaskName, period: $scope.Period, frequency: $scope.Frequency, date: null, duration: $scope.Duration });
            }
            Clear();
        }

        var inputField = $window.document.getElementById("taskInputField");
        inputField.focus();
    }

    $scope.AddOnTheGo = function () {
        var taskId = GenerateGuid();

        var task = { Id: taskId, Name: $scope.TaskName, Type: "OnTheGo" }
        $scope.Tasks.push(task);

        $scope.Generating = true;
        $http.post("Home/AddOnTheGo", { id: taskId, name: $scope.TaskName }).success(AddScheduledOnTheGo);
        Clear();
        var inputField = $window.document.getElementById("taskInputField");
        inputField.focus();
    }

    $scope.TimeBased = function () {
        $scope.TimeBased = !$scope.TimeBased;
    }

    $scope.AddEvent = function () {
        $scope.ShowAdvanced = !$scope.ShowAdvanced;
    }

    $scope.AddRecurring = function () {
        $scope.ShowRecurring = !$scope.ShowRecurring;
    }

    function Clear() {
        $scope.TaskName = "";
        $scope.Period = "";
        $scope.Frequency = "";
        $scope.Duration = "";
        $scope.ShowAdvanced = false;
        $scope.ShowRecurring = false;
        $scope.TimeBased = true;
    }

    $scope.CompleteTask = function (task) {
        $http.post("Home/CompleteTask", { id: task.Id });
        task.TaskStatus = !task.TaskStatus;
        task.Locked = task.TaskStatus;
    }

    $scope.Lock = function (task) {
        $http.post("Home/LockTask", { id: task.Id });
        task.Locked = !task.Locked;
    }

    $scope.RemoveTask = function (taskId, index) {
        $scope.Tasks.splice(index, 1);
        $http.post("Home/Remove", { id: taskId.Id });

        var inputField = $window.document.getElementById("taskInputField");
        inputField.focus();
    }

    $scope.GenerateSchedule = function () {
        $scope.Generating = true;
        $http.post("Home/AddSchedule", { from: $scope.From, to: $scope.To }).success(AddScheduledTasks);

        var inputField = $window.document.getElementById("taskInputField");
        inputField.focus();
    }

    $scope.CancelAdd = function () {
        var name = $scope.TaskName;
        Clear();
        $scope.TaskName = name;
        var inputField = $window.document.getElementById("taskInputField");
        inputField.focus();
    }

    $scope.EditTask = function (taskId, index) {
        $scope.Items = taskId;
        var modalInstance = $uibModal.open({
            animation: $scope.animationsEnabled,
            templateUrl: 'myModalContent.html',
            controller: 'ModalInstanceCtrl',
            size: 'sm',
            resolve: {
                items: function () {
                    return $scope.Items;
                }
            }
        });
    }

    function S4() {
        return (((1 + Math.random()) * 0x10000) | 0).toString(16).substring(1);
    }

    function GenerateGuid() {
        return (S4() + S4() + "-" + S4() + "-4" + S4().substr(0, 3) + "-" + S4() + "-" + S4() + S4() + S4()).toLowerCase();
    }

    function AddScheduledTasks(data) {
        $scope.ScheduledTasks = data.Data;
        for (var i = 0; i < $scope.ScheduledTasks.length; i++) {
            var date = $scope.ScheduledTasks[i].Date;
            $scope.ScheduledTasks[i].Date = new Date(parseInt(date.replace('/Date(', '')));
        }
        $scope.Generating = false;
    };

    function AddScheduledOnTheGo(data) {
        $scope.ScheduledTasks = data.Data;
        for (var i = 0; i < $scope.ScheduledTasks.length; i++) {
            var date = $scope.ScheduledTasks[i].Date;
            $scope.ScheduledTasks[i].Date = new Date(parseInt(date.replace('/Date(', '')));
        }
        $scope.Generating = false;
    }
});

angular.module('taskApp').controller('ModalInstanceCtrl', function ($scope, $uibModalInstance, $http, items) {
    $scope.Task = items;
    $scope.TaskName = $scope.Task.Name;
    $scope.Duration = $scope.Task.Hours;

    $scope.ok = function () {
        $scope.Task.Name = $scope.TaskName;
        $scope.Task.Duration = $scope.Duration;
        $http.post("Home/Edit", { id: $scope.Task.Id, name: $scope.Task.Name, duration : $scope.Task.Duration });
        $uibModalInstance.close($scope.Task);
    };

    $scope.cancel = function () {
        $uibModalInstance.dismiss('cancel');
    };
});