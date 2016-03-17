'use strict';

var app = angular.module('deepheadz', ['ngRoute', 'ui.bootstrap', 'uiGmapgoogle-maps'])
    .controller('locationController', function($scope, $location) {
        $scope.location = {latitude: 55.7522200, longitude: 37.6155600};
        $scope.radius = 2600;
        $scope.map = {
            center: $scope.location,
            zoom: 12,
            events: {
                click: function(map, eventName, originalEventArgs) {
                    var e = originalEventArgs[0];
                    $scope.location = {
                        latitude: e.latLng.lat(),
                        longitude: e.latLng.lng()
                    };
                    if (!$scope.$digest)
                        $scope.$apply();
                }
            }
        };

        $scope.goToRooms = function() {
            $location.path('/rooms').search({
                checkIn: $scope.checkIn,
                checkOut: $scope.checkOut,
                minAvailabilityRatio: $scope.minAvailabilityRatio,
                latitude: $scope.location.latitude,
                longitude: $scope.location.longitude,
                radius: $scope.radius / 1000
            });
        };
        if (!$scope.$digest)
            $scope.$apply();
    })
    .controller('roomsController', function($scope, $http, $routeParams) {
        $http.get(
            '//localhost:8080/api/rooms',
            {
                params: {
                    longitude: $routeParams.longitude,
                    latitude: $routeParams.latitude,
                    radius: $routeParams.radius,
                    checkIn: $routeParams.checkIn,
                    checkOut: $routeParams.checkOut,
                    minAvailabilityRatio: $routeParams.minAvailabilityRatio
                }
            }).then(onRoomsSuccess, console.log);

        function onRoomsSuccess(response) {
            console.log(response);
            $scope.rooms = response.data.rooms;
            $scope.totalNumberOfRooms = response.data.totalNumberOfRooms;
            $scope.totalNumberOfGuests = response.data.totalNumberOfGuests;
            $scope.averageAvailability = response.data.averageAvailability;
            if (!$scope.$digest)
                $scope.$apply();
        }

        $scope.getImages = function(room) {
            var index = 0;
            return room.images.map(function(image) {
                return {index: index++, image: image};
            });
        };
    });

app.config(['$routeProvider',
    function($routeProvider) {
        $routeProvider.when('/', {
            templateUrl: 'html/locations.html',
            controller: 'locationController'
        }).when('/locations', {
            templateUrl: 'html/locations.html',
            controller: 'locationController'
        }).when('/rooms', {
            templateUrl: 'html/rooms.html',
            controller: 'roomsController'
        }).otherwise({
            redirectTo: '/'
        });
    }]);