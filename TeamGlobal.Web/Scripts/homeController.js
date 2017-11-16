$(document).ready(function () {
    //$('#divname').load("/Home/GetSearchResult");
    var numberOfDaysToAdd = 56;
    //$("#FromDate").datepicker({
    $("#infoBar0").hide();
    $("#infoBar1").hide();

    Date.prototype.mmddyyyy = function () {
        var yyyy = this.getFullYear();
        var mm = this.getMonth() < 9 ? "0" + (this.getMonth() + 1) : (this.getMonth() + 1); // getMonth() is zero-based
        var dd = this.getDate() < 10 ? "0" + this.getDate() : this.getDate();
        return "" + mm + "-" + dd + "-" + yyyy;
    };

    Date.prototype.yyyymmdd = function () {
        var yyyy = this.getFullYear();
        var mm = this.getMonth() < 9 ? "0" + (this.getMonth() + 1) : (this.getMonth() + 1); // getMonth() is zero-based
        var dd = this.getDate() < 10 ? "0" + this.getDate() : this.getDate();
        return "" + yyyy + "-" + mm + "-" + dd;
    };

    $("#FromDate").datepicker({
        dateFormat: 'yy-mm-dd',
        onSelect: function (dateText) {
            //alert("Selected date: " + dateText + "; input's current value: " + this.value);
            var toDate = new Date(dateText);

            toDate.setDate(toDate.getDate() + numberOfDaysToAdd);

            $("#ToDate").val(toDate.yyyymmdd());
        }
    });  // For previous month's date

    $("#ToDate").datepicker({
        dateFormat: 'yy-mm-dd',
        onSelect: function (dateText) {
            var fromDate = $("#FromDate").val();

            var toDate = new Date(fromDate);

            toDate.setDate(toDate.getDate() + numberOfDaysToAdd);

            $("#ToDate").val(toDate.mmddyyyy());
        }
    }); // For current date

    var fromDate = $("#FromDate").val();
    var toDate = $("#ToDate").val();
    var fromSource = $("#OriginId").val();
    var toDestination = $("#DestinationId").val();

    function IsLocationIsInValid(inputString, subString) {
        return (inputString.indexOf(subString) !== -1);
    }

    function isEmpty(value) {
        return (value === null || value.length === 0);
    }

    $(".date").datepicker({
        onSelect: function (dateText) {
            display("Selected date: " + dateText + "; input's current value: " + this.value);
        }
    });

    $(document).on("click", "#donwloadExcel", function (e) {
        var from = $("#FromDate").val();
        var fromDate = new Date(from).yyyymmdd();

        var to = $("#ToDate").val();
        var toDate = new Date(to).yyyymmdd();

        var fromSource = $("#OriginId").val();
        var fromDestination = $("#DestinationId").val();

        $('body').loading({
            theme: 'light',
            message: 'Loading...'
        });

        $.ajax({
            type: "GET",
            url: '/Home/DummyCall/',
            datatype: "text/plain",
            data: {
                'fromDate': fromDate,
                'toDate': toDate,
                'fromSource': fromSource,
                'toDestination': fromDestination
            },
            cache: false,
            success: function (data) {
                window.location.href = "/Home/ExportExcel?fromDate=" + fromDate + "&toDate=" + toDate + "&fromSource=" + fromSource + "&toDestination=" + fromDestination;

                setTimeout(function () {
                    $('body').loading('stop');
                }, 1000);
            }, // Success
            error: function (req) {
                setTimeout(function () {
                    $('body').loading('stop');
                }, 1000);
            } // Fail
        });
    });

    $("#btnSearch").click(function () {
        var from = $("#FromDate").val();
        var fromDate = new Date(from).yyyymmdd();

        var to = $("#ToDate").val();
        var toDate = new Date(to).yyyymmdd();

        var fromSource = $("#OriginId").val();
        var toDestination = $("#DestinationId").val();

        var source = $("#FromOrigin").val();
        var destination = $("#ToDestination").val();

        $('#divname').html('');
        $("#infoBar0").hide();
        $("#infoBar1").hide();

        $('body').loading({
            theme: 'light',
            message: 'Loading...'
        });

        validateVesselInfo(source, destination, fromSource, toDestination);
        getVesselInfo(fromDate, toDate, fromSource, toDestination);
    });

    $("#FromOrigin").autocomplete({
        source: function (request, response) {
            $.ajax({
                url: '/Home/SourceLocation/',
                type: "POST",
                dataType: "json",
                data: { sourceLocation: request.term },
                success: function (data) {
                    response($.map(data, function (val, item) {
                        return {
                            label: val.Name,
                            value: val.Code,
                            codeName: val.Value,
                            countryName: val.Country
                        };
                    }));
                },
                error: function (response) {
                    alert(response.responseText);
                },
                failure: function (response) {
                    alert(response.responseText);
                }
            });
        },
        select: function (event, ui) {
            $("#OriginId").val(ui.item.codeName);
            $("#Origin").val('');
            $("#Origin").val(ui.item.countryName);
        }
    });

    $("#ToDestination").autocomplete({
        source: function (request, response) {
            $.ajax({
                url: '/Home/DestinationLocation',
                type: "POST",
                dataType: "json",
                data: { destinationLocation: request.term },
                success: function (data) {
                    response($.map(data, function (val, item) {
                        return {
                            label: val.Name,
                            value: val.Code,
                            codeName: val.Value,
                            countryName: val.Country
                        };
                    }));
                },
                error: function (response) {
                    alert(response.responseText);
                },
                failure: function (response) {
                    alert(response.responseText);
                }
            });
        },
        select: function (event, ui) {
            $("#DestinationId").val(ui.item.codeName);
            $("#Destination").val('');
            $("#Destination").val(ui.item.countryName);
        }
    });

    function setDefaultDate(noOfDays) {
        var defaultDate = new Date();
        defaultDate.setDate(defaultDate.getDate() + noOfDays);
        var yy = defaultDate.getFullYear();

        var mm = ("0" + (defaultDate.getMonth() + 1)).slice(-2);

        var dd = ((defaultDate.getDate() < 10) ? "0" : "") + defaultDate.getDate();

        return "" + yy + "-" + mm + "-" + dd;
    }

    function getAddress(sourceCode, destinationCode, isGetRequest) {
        //show loading... image

        $.ajax({
            type: "GET",
            url: '/Home/GetLoactionInfo/',
            datatype: "text/plain",
            data: {
                'sourceCode': sourceCode,
                'destinationCode': destinationCode
            },
            cache: false,
            success: function (data, textStatus) {
                var originName = data[0].NAME;
                var originCountry = data[0].CountryName;
                var originCode = data[0].CODE;

                var destinationName = data[1].NAME;
                var destinationCountry = data[1].CountryName;
                var destinationCode = data[1].CODE;
                if (isGetRequest) {
                    $("#lblOrigin").text(originName + ", " + originCountry);
                    $("#lblDestination").text(destinationName + ", " + destinationCountry);
                } else {
                    $("#Origin").val(originCountry);
                    $("#Destination").val(destinationCountry);

                    $("#FromOrigin").val(originCode + ", " + originName);
                    $("#ToDestination").val(destinationCode + ", " + destinationName);
                }
            }, //success
            error: function (xhr, textStatus, errorThrown) {
                console.log('error');
            } // error
        });
    }

    if (!isEmpty(fromDate) && !isEmpty(toDate) && !isEmpty(fromSource) && !isEmpty(toDestination)) {
        var from = $("#FromDate").val();
        var fromDate = new Date(from).yyyymmdd();

        var to = $("#ToDate").val();
        var toDate = new Date(to).yyyymmdd();

        var fromSource = $("#OriginId").val();
        var toDestination = $("#DestinationId").val();

        var source = $("#FromOrigin").val();
        var destination = $("#ToDestination").val();

        $('#divname').html('');

        $("#infoBar0").hide();
        $("#infoBar1").hide();

        $('body').loading({
            theme: 'light',
            message: 'Loading...'
        });

        getVesselInfo(fromDate, toDate, fromSource, toDestination);

        getAddress(fromSource, toDestination, false);
        //$("#btnSearch").trigger("click");
        //$('#btnSearch').click();
    } else {
        $("#FromDate").val(setDefaultDate(0));
        $("#ToDate").val(setDefaultDate(numberOfDaysToAdd));
    }

    function getVesselInfo(fromDate, toDate, fromSource, toDestination) {
        $.ajax({
            type: "GET",
            url: '/Home/GetSearchResult/',
            datatype: "text/plain",
            data: {
                'fromDate': fromDate,
                'toDate': toDate,
                'fromSource': fromSource,
                'toDestination': toDestination
            },
            cache: false,
            success: function (data, textStatus) {
                $('#divname').html(data);

                getAddress(fromSource, toDestination, true);
                setTimeout(function () {
                    $('body').loading('stop');
                   

                }, 1000);

                $("#infoBar0").show();
                $("#infoBar1").show();
            }, //success
            error: function (req) {
                setTimeout(function () {
                    $('body').loading('stop');
                }, 1000);
            }
        });
    }

    function validateVesselInfo(source, destination, fromSource, toDestination) {
        if (isEmpty(source)) {
            swal("Origin is Empty Or Not Selected", "Please Select the Origin.", "warning");
            return;
        }

        if (isEmpty(destination)) {
            swal("Destination is Empty Or Not Selected", "Please Select the Destination.", "warning");
            return;
        }

        if (IsLocationIsInValid(source, fromSource) == false) {
            swal("Origin Error", "You have not selected Origin Properly,\n Plese select it again.", "error");
            return;
        }

        if (IsLocationIsInValid(destination, toDestination) == false) {
            swal("Destination Error", "You have not selected Origin Properly,\n Plese select it again.", "error");
            return;
        }
    }
    function bookNow() {
        alert('yes');
    }

    $(document).on("click", "#btnSendQuery", function (e) {
        var fromDate = $("#FromDate").val();
        var toDate = $("#ToDate").val();
        var fromOrigin = $("#OriginId").val();
        var toDestination = $("#DestinationId").val();
        var origin = $("#lblOrigin").text();
        var destination = $("#lblDestination").text();
        var firstName = $("#firstName").val();
        var lastName = $("#lastName").val();
        var email = $("#email").val();
        var phone = $("#phone").val();
        var selectedValue = $("#SelectedValue").val();
        var errorCount = 0;

        if (firstName == null || firstName.length < 3) {
            $("#divFirstName").removeClass("has-success");
            $("#spanFirstName").removeClass("glyphicon-ok");
            $("#divFirstName").addClass("has-error");
            $("#spanFirstName").addClass("glyphicon-remove");
            errorCount++;
        } else {
            $("#divFirstName").removeClass("has-error");
            $("#spanFirstName").removeClass("glyphicon-remove");
            $("#divFirstName").addClass("has-success");
            $("#spanFirstName").addClass("glyphicon-ok");
        }

        if (lastName == null || lastName.length < 3) {
            $("#divLastName").removeClass("has-success");
            $("#spanLastName").removeClass("glyphicon-ok");
            $("#divLastName").addClass("has-error");
            $("#spanLastName").addClass("glyphicon-remove");
            errorCount++;
        } else {
            $("#divLastName").removeClass("has-error");
            $("#spanLastName").removeClass("glyphicon-remove");
            $("#divLastName").addClass("has-success");
            $("#spanLastName").addClass("glyphicon-ok");
        }

        if (phone == null || phone.length > 13 || phone.length < 8) {
            $("#divPhone").removeClass("has-success");
            $("#spanPhone").removeClass("glyphicon-ok");
            $("#divPhone").addClass("has-error");
            $("#spanPhone").addClass("glyphicon-remove");
            errorCount++;
        } else {
            $("#divPhone").removeClass("has-error");
            $("#spanPhone").removeClass("glyphicon-remove");
            $("#divPhone").addClass("has-success");
            $("#spanPhone").addClass("glyphicon-ok");
        }

        if (email == null || email.length < 5 || !validateEmail(email)) {
            $("#divEmail").removeClass("has-success");
            $("#spanEmail").removeClass("glyphicon-ok");
            $("#divEmail").addClass("has-error");
            $("#spanEmail").addClass("glyphicon-remove");
            errorCount++;
        } else {
            $("#divEmail").removeClass("has-error");
            $("#spanEmail").removeClass("glyphicon-remove");
            $("#divEmail").addClass("has-success");
            $("#spanEmail").addClass("glyphicon-ok");
        }
        

        if (errorCount != 0)
            return;
        $("#btnSendQuery").addClass("disabled");
        
        $.ajax({
            type: "POST",
            url: "/Home/SubmitQuery/",
            //contentType: "application/json; charset=utf-8",
            data: {
                "FromDate": fromDate,
                "ToDate": toDate,
                "FromOrigin": fromOrigin,
                "ToDestination": toDestination,
                "Origin": origin,
                "Destination": destination,
                "FirstName": firstName,
                "LastName": lastName,
                "Phone": phone,
                "Email": email,
                "SelectedValue": selectedValue
            },
            datatype: "json",
            success: function (data) {
                $('#bookNowModal').modal('hide');
                $('#successModal').modal('show');
                $("#btnSendQuery").removeClass("disabled");
            },
            error: function () {
                alert("Dynamic content load failed.");
                $("#btnSendQuery").removeClass("disabled");
            }
        });
       
    });

    function validateEmail(email) {
        var re = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
        return re.test(email);
    }

    $(document).on("click", "#bookNow", function (e) {
        var fromDate = $("#FromDate").val();
        var toDate = $("#ToDate").val();
        var fromOrigin = $("#OriginId").val();
        var toDestination = $("#DestinationId").val();

        var origin = $("#lblOrigin").text();
        var destination = $("#lblDestination").text();

        var selectedValue = $(this).attr("data-result");

        $('#bookNowModalContent').html("");
        $.ajax({
            type: "GET",
            url: "/Home/QueryForm/",
            //contentType: "application/json; charset=utf-8",
            data: {
                "FromDate": fromDate,
                "ToDate": toDate,
                "FromOrigin": fromOrigin,
                "ToDestination": toDestination,
                "Origin": origin,
                "Destination": destination,
                "SelectedValue": selectedValue
            },
            datatype: "json",
            success: function (data) {
                //debugger;
                $('#bookNowModalContent').html(data);
                //$('#myModal').modal(options);
                $('#bookNowModal').modal('show');
            },
            error: function () {
                alert("Dynamic content load failed.");
            }
        });
    });
});