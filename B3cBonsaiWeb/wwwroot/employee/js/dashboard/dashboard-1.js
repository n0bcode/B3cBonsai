﻿

(function($) {
    /* "use strict" */
	
	var dzChartlist = function () {

		var screenWidth = $(window).width();
		let draw = Chart.controllers.line.__super__.draw; //draw shadow

		var NewCustomers = function () {
			var options = {
				series: [
					{
						name: 'Net Profit',
						data: [100, 300, 200, 250, 200, 240, 180, 230, 200, 250, 300],
						/* radius: 30,	 */
					},
				],
				chart: {
					type: 'area',
					height: 40,
					//width: 400,
					toolbar: {
						show: false,
					},
					zoom: {
						enabled: false
					},
					sparkline: {
						enabled: true
					}

				},

				colors: ['var(--primary)'],
				dataLabels: {
					enabled: false,
				},

				legend: {
					show: false,
				},
				stroke: {
					show: true,
					width: 2,
					curve: 'straight',
					colors: ['var(--primary)'],
				},

				grid: {
					show: false,
					borderColor: '#eee',
					padding: {
						top: 0,
						right: 0,
						bottom: 0,
						left: -1

					}
				},
				states: {
					normal: {
						filter: {
							type: 'none',
							value: 0
						}
					},
					hover: {
						filter: {
							type: 'none',
							value: 0
						}
					},
					active: {
						allowMultipleDataPointsSelection: false,
						filter: {
							type: 'none',
							value: 0
						}
					}
				},
				xaxis: {
					categories: ['Jan', 'feb', 'Mar', 'Apr', 'May', 'June', 'July', 'August', 'Sept', 'Oct'],
					axisBorder: {
						show: false,
					},
					axisTicks: {
						show: false
					},
					labels: {
						show: false,
						style: {
							fontSize: '12px',

						}
					},
					crosshairs: {
						show: false,
						position: 'front',
						stroke: {
							width: 1,
							dashArray: 3
						}
					},
					tooltip: {
						enabled: true,
						formatter: undefined,
						offsetY: 0,
						style: {
							fontSize: '12px',
						}
					}
				},
				yaxis: {
					show: false,
				},
				fill: {
					opacity: 0.9,
					colors: 'var(--primary)',
					type: 'gradient',
					gradient: {
						colorStops: [

							{
								offset: 0,
								color: 'var(--primary)',
								opacity: .4
							},
							{
								offset: 0.6,
								color: 'var(--primary)',
								opacity: .4
							},
							{
								offset: 100,
								color: 'white',
								opacity: 0
							}
						],

					}
				},
				tooltip: {
					enabled: false,
					style: {
						fontSize: '12px',
					},
					y: {
						formatter: function (val) {
							return "$" + val + " thousands"
						}
					}
				}
			};

			var chartBar1 = new ApexCharts(document.querySelector("#NewCustomers"), options);
			chartBar1.render();

		}
		var NewExperience = function () {
			var options = {
				series: [
					{
						name: 'Net Profit',
						data: [100, 300, 200, 250, 200, 240, 180, 230, 200, 250, 300],
						/* radius: 30,	 */
					},
				],
				chart: {
					type: 'area',
					height: 40,
					//width: 400,
					toolbar: {
						show: false,
					},
					zoom: {
						enabled: false
					},
					sparkline: {
						enabled: true
					}

				},

				colors: ['var(--primary)'],
				dataLabels: {
					enabled: false,
				},

				legend: {
					show: false,
				},
				stroke: {
					show: true,
					width: 2,
					curve: 'straight',
					colors: ['#FF5E5E'],
				},

				grid: {
					show: false,
					borderColor: '#eee',
					padding: {
						top: 0,
						right: 0,
						bottom: 0,
						left: -1

					}
				},
				states: {
					normal: {
						filter: {
							type: 'none',
							value: 0
						}
					},
					hover: {
						filter: {
							type: 'none',
							value: 0
						}
					},
					active: {
						allowMultipleDataPointsSelection: false,
						filter: {
							type: 'none',
							value: 0
						}
					}
				},
				xaxis: {
					categories: ['Jan', 'feb', 'Mar', 'Apr', 'May', 'June', 'July', 'August', 'Sept', 'Oct'],
					axisBorder: {
						show: false,
					},
					axisTicks: {
						show: false
					},
					labels: {
						show: false,
						style: {
							fontSize: '12px',
						}
					},
					crosshairs: {
						show: false,
						position: 'front',
						stroke: {
							width: 1,
							dashArray: 3
						}
					},
					tooltip: {
						enabled: true,
						formatter: undefined,
						offsetY: 0,
						style: {
							fontSize: '12px',
						}
					}
				},
				yaxis: {
					show: false,
				},
				fill: {
					opacity: 0.9,
					colors: '#FF5E5E',
					type: 'gradient',
					gradient: {
						colorStops: [

							{
								offset: 0,
								color: '#FF5E5E',
								opacity: .5
							},
							{
								offset: 0.6,
								color: '#FF5E5E',
								opacity: .5
							},
							{
								offset: 100,
								color: 'white',
								opacity: 0
							}
						],

					}
				},
				tooltip: {
					enabled: false,
					style: {
						fontSize: '12px',
					},
					y: {
						formatter: function (val) {
							return "$" + val + " thousands"
						}
					}
				}
			};

			var chartBar1 = new ApexCharts(document.querySelector("#NewExperience"), options);
			chartBar1.render();

		};
		var AllProject = function () {
			$.ajax({
				url: '/employee/dashboard/GetAllOrderData', // Đảm bảo URL chính xác
				method: 'GET',
				success: function (response) {
					var options = {
						series: [response.approved, response.pending, response.inAllProgress],
						chart: {
							type: 'donut',
							width: 150,
						},
						plotOptions: {
							pie: {
								donut: {
									size: '80%',
									labels: {
										show: true,
										name: {
											show: true,
											offsetY: 12,
										},
										value: {
											show: true,
											fontSize: '22px',
											fontFamily: 'Arial',
											fontWeight: '500',
											offsetY: -17,
										},
										total: {
											show: true,
											fontSize: '11px',
											fontWeight: '500',
											fontFamily: 'Arial',
											label: 'Tổng số đơn',
											formatter: function (w) {
												return w.globals.seriesTotals.reduce((a, b) => {
													return a + b
												}, 0);
											}
										}
									}
								}
							}
						},
						legend: {
							show: false,
						},
						colors: ['#3AC977', 'var(--primary)', 'var(--secondary)'],
						labels: ["Đã nhận", "Đang xử lí", "Trạng thái khác"],
						dataLabels: {
							enabled: false,
						},
					};

					var chartBar1 = new ApexCharts(document.querySelector("#AllProject"), options);
					chartBar1.render();
				},
				error: function (xhr, status, error) {
					console.error("Error fetching data: ", error);
				}
			});
		};

		var overiewChart = function () {
			var fetchDataByStatus = function (status, timeRange) {
				return $.ajax({
					url: '/Employee/Dashboard/GetOrderOverViewData',
					method: 'GET',
					data: { timeRange: timeRange },
					success: function (response) {
						var data = response.data.find(item => item.name === status);
						if (data && Array.isArray(data.data)) {
							return data.data;  // Trả về mảng dữ liệu nếu hợp lệ
						} else {
							return []; // Trả về mảng rỗng nếu không có dữ liệu hợp lệ
						}
					}
				});
			};

			// Đảm bảo rằng dữ liệu trả về là mảng và hợp lệ
			var options = {
				series: [
					{
						name: 'Đang giao',
						type: 'column',
						data: [] // Mảng trống ban đầu, sẽ cập nhật sau khi có dữ liệu
					},
					{
						name: 'Đang xử lí',
						type: 'area',
						data: []
					},
					{
						name: 'Đã hủy',
						type: 'line',
						data: []
					},
					{
						name: 'Đã giao',
						type: 'line',
						data: []
					},
					{
						name: 'Đã nhận',
						type: 'line',
						data: []
					}
				],
				chart: {
					height: 300,
					type: 'line',
					stacked: false,
					toolbar: { show: false },
				},
				stroke: {
					width: [0, 1, 1, 1, 1],
					curve: 'straight',
					dashArray: [0, 0, 0, 0, 0]
				},
				legend: {
					fontSize: '13px',
					fontFamily: 'poppins',
					labels: {
						colors: '#888888',
					}
				},
				plotOptions: {
					bar: {
						columnWidth: '18%',
						borderRadius: 6,
					}
				},
				fill: {
					type: 'gradient',
					gradient: {
						inverseColors: false,
						shade: 'light',
						type: "vertical",
						stops: [0, 100, 100, 100]
					}
				},
				colors: ["#007bff", "#3AC977", "#FF5E5E", "#00C7E6", "#FFC107"],
				labels: [], // Sẽ được cập nhật từ API
				markers: {
					size: 0
				},
				xaxis: {
					type: 'category',
					categories: [], // Sẽ được cập nhật từ API
				},
				yaxis: {
					min: 0,
					tickAmount: 4,
				},
				tooltip: {
					shared: true,
					y: {
						formatter: function (y) {
							if (typeof y !== "undefined") {
								return y.toFixed(0) + " đơn hàng";
							}
							return y;
						}
					}
				}
			};

			var chart = new ApexCharts(document.querySelector("#overiewChart"), options);
			chart.render();

			// Lấy và cập nhật dữ liệu từ API khi tải trang hoặc thay đổi thời gian
			function updateChartData(timeRange) {
				$.ajax({
					url: '/Employee/Dashboard/GetOrderOverViewData',
					method: 'GET',
					data: { timeRange: timeRange },
					success: function (response) {
						var seriesData = response.data;
						var categories = response.categories; // Nhận categories từ API

						$('#amountOrders').text(response.amountOrders);
						$('#amountSuccessOrders').text(response.amountSuccessOrders);

						if (Array.isArray(seriesData) && seriesData.length > 0) {
							chart.updateOptions({
								xaxis: {
									categories: categories // Cập nhật categories cho trục x
								},
								labels: categories // Cập nhật labels cho trục x
							});

							chart.updateSeries(seriesData.map(function (item) {
								return {
									name: item.nameVietNamese,
									type: item.name === 'Processing' ? 'column' : 'line', // Sử dụng column cho Processing, line cho các trạng thái còn lại
									data: item.data
								};
							}));
						}
					}
				});
			}

			// Lắng nghe sự kiện thay đổi tab hoặc thời gian
			$(".mix-chart-tab .nav-link").on('click', function () {
				var timeRange = $(this).attr('data-time-range'); // Lấy khoảng thời gian từ tab
				updateChartData(timeRange); // Cập nhật dữ liệu theo thời gian đã chọn
			});

			// Cập nhật dữ liệu cho biểu đồ lần đầu tiên khi trang được tải
			updateChartData("day"); // Mặc định là "day"
		};


		var earningChart = function () {

			var chartWidth = $("#earningChart").width();

			var options = {
				series: [
					{
						name: 'Net Profit',
						data: [], // Start with an empty array, will be populated by AJAX
					},
				],
				chart: {
					type: 'area',
					height: 350,
					width: chartWidth + 55,
					toolbar: {
						show: false,
					},
					offsetX: -45,
					zoom: {
						enabled: false
					}
				},
				colors: ['var(--primary)'],
				dataLabels: {
					enabled: false,
				},
				legend: {
					show: false,
				},
				stroke: {
					show: true,
					width: 2,
					curve: 'smooth',
					colors: ['var(--primary)'],
				},
				grid: {
					show: true,
					borderColor: '#eee',
					xaxis: {
						lines: {
							show: true
						}
					},
					yaxis: {
						lines: {
							show: true
						}
					},
				},
				yaxis: {
					show: true,
					tickAmount: 5,
					min: 0,
					max: 12000000, // Adjusted max value for better visibility
					labels: {
						offsetX: 50,
						style: {
							fontSize: '12px',
							fontWeight: 600,
						}
					}
				},
				xaxis: {
					categories: [], // This will be populated dynamically based on the time range
					axisBorder: {
						show: false,
					},
					axisTicks: {
						show: false
					},
					labels: {
						show: true,
						offsetX: 5,
						style: {
							fontSize: '12px',
						}
					},
				},
				fill: {
					opacity: 0.5,
					colors: 'var(--primary)',
					type: 'gradient',
					gradient: {
						colorStops: [
							{
								offset: 0.6,
								color: 'var(--primary)',
								opacity: .2
							},
							{
								offset: 0.6,
								color: 'var(--primary)',
								opacity: .15
							},
							{
								offset: 100,
								color: 'white',
								opacity: 0
							}
						],
					}
				},
				tooltip: {
					enabled: true,
					style: {
						fontSize: '12px',
					},
					y: {
						formatter: function (val) {
							return val.toLocaleString('VN-EN') + 'đ'; // Format to show 2 decimal places
						}
					}
				}
			};

			var chartBar1 = new ApexCharts(document.querySelector("#earningChart"), options);
			chartBar1.render();

			// Function to fetch data from the backend and update the chart
			function fetchData(timeRange) {
				$.ajax({
					url: '/Employee/Dashboard/GetEarningData', // Ensure the correct URL
					type: 'GET',
					data: { timeRange: timeRange },
					success: function (response) {
						// Update chart series with data from backend
						chartBar1.updateSeries([{
							name: response.name,
							data: response.data
						}]);

						// Update x-axis categories based on time range
						chartBar1.updateOptions({
							xaxis: {
								categories: response.categories // Use categories from response
							}
						});
					},
					error: function (xhr, status, error) {
						console.error('Error fetching data: ', error);
					}
				});
			}

			// Initial data load for 'day' (you can change this to any default time range)
			fetchData('day');

			// Event listener for changing the time range
			$(".earning-chart .nav-link").on('click', function () {
				var seriesType = $(this).attr('data-series');
				fetchData(seriesType); // Fetch new data based on selected time range
			});
		};
	
		var projectChart = function (timeRange) {
			// Kiểm tra và xoá biểu đồ cũ nếu có
			if (window.chart) {
				window.chart.destroy();
			}

			// Gọi API để lấy dữ liệu mới dựa trên timeRange
			fetch(`/Employee/Dashboard/GetOrderStatusData?timeRange=${timeRange}`)
				.then(response => response.json())
				.then(data => {
					var options = {
						series: data.data,
						chart: {
							type: 'donut',
							width: 250,
						},
						labels: data.labels,
						colors: ['#FF9F00', 'var(--primary)', '#3AC977', '#FF5E5E', '#ffb8c6'],
						plotOptions: {
							pie: {
								donut: {
									size: '90%',
									labels: {
										show: true,
										name: {
											show: true,
											offsetY: 12,
										},
										value: {
											show: true,
											fontSize: '24px',
											fontFamily: 'Arial',
											fontWeight: '500',
											offsetY: -17,
										},
										total: {
											show: true,
											fontSize: '11px',
											fontWeight: '500',
											fontFamily: 'Arial',
											label: 'Tổng số trạng thái đơn hàng ',
											formatter: function (w) {
												return w.globals.seriesTotals.reduce((a, b) => a + b, 0);
											}
										}
									}
								}
							}
						},
						legend: {
							show: false,
						}
					};

					// Khởi tạo và lưu biểu đồ mới vào biến window.chart
					window.chart = new ApexCharts(document.querySelector("#projectChart"), options);
					window.chart.render();

					// Đưa thông số ra hiển thị
					$('#amountInProcess').html(data.data[0]);
					$('#amountPending').html(data.data[1]);
					$('#amountCancelled').html(data.data[2]);
					$('#amountShipped').html(data.data[3]);
					$('#amountApproved').html(data.data[4]);
				});
		};

		// Thêm sự kiện cho phần chọn thời gian
		document.querySelector(".status-select").addEventListener("change", function () {
			var timeRange = this.value.toLowerCase();
			projectChart(timeRange);
		});

		// Tạo biểu đồ ban đầu với giá trị "today"
		projectChart("today");

	/*var handleWorldMap = function(trigger = 'load'){
		var vmapSelector = $('#world-map');
		if(trigger == 'resize')
		{
			vmapSelector.empty();
			vmapSelector.removeAttr('style');
		}
		
		vmapSelector.delay( 500 ).unbind().vectorMap({ 
			map: 'world_en',
			backgroundColor: 'transparent',
			borderColor: 'rgb(239, 242, 244)',
			borderOpacity: 0.25,
			borderWidth: 1,
			color: 'rgb(239, 242, 244)',
			enableZoom: true,
			hoverColor: 'rgba(239, 242, 244 0.9)',
			hoverOpacity: null,
			normalizeFunction: 'linear',
			scaleColors: ['#b6d6ff', '#005ace'],
			selectedColor: 'rgba(239, 242, 244 0.9)',
			selectedRegions: null,
			showTooltip: true,
			onRegionClick: function(element, code, region)
			{
				var message = 'You clicked "'
					+ region
					+ '" which has the code: '
					+ code.toUpperCase();
		 
				alert(message);
			}
		});
	}*/
	/* Function ============ */
		return {
			init:function(){
			},
			
			
			load:function(){
				NewCustomers();
				NewExperience();
				AllProject();
				overiewChart();
				earningChart();
				projectChart();
				/*handleWorldMap();*/
				
			},
			
			resize:function(){
				handleWorldMap();
				earningChart();
			}
		}
	
	}();

	
		
	jQuery(window).on('load',function(){
		setTimeout(function(){
			dzChartlist.load();
		}, 1000); 
		
	});

     

})(jQuery);
