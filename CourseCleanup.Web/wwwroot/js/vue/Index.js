Vue.use(Vuetable);

Array.prototype.any = function (callback) { return this.findIndex(callback) > -1; };

$(document).ready(function () {
    var app = new Vue({
        el: "#app",
        components: {
            Multiselect: window.VueMultiselect.default
        },
        data: {
            startTerm: "",
            endTerm: "",
            termsValue: [],
            termsOptions: [],
            courseSearchStatusTableFields: [
                {
                    name: 'DateCreated',
                    title: 'Date Created',
                    sortField: 'DateCreated',
                    filterable: true,
                    visible: true,
                    formatter: (value) => {
                        if (!value) return "";
                        return moment(value).format('M/D/YY h:mm a');
                    }
                },
                {
                    name: 'LastUpdated',
                    title: 'Last Updated',
                    sortField: 'LastUpdated',
                    filterable: true,
                    visible: true,
                    formatter: (value) => {
                        if (!value) return "";
                        return moment(value).format('M/D/YY h:mm a');
                    }
                },
                //{
                //    name: 'StartTerm',
                //    title: 'Start Term',
                //    sortField: 'StartTerm',
                //    filterable: true,
                //    visible: true
                //},
                //{
                //    name: 'EndTerm',
                //    title: 'End Term',
                //    sortField: 'EndTerm',
                //    filterable: true,
                //    visible: true
                //},
                {
                    name: 'TermList',
                    title: 'Terms',
                    sortField: 'TermList',
                    filterable: true,
                    visible: true
                },
                {
                    name: 'SubmittedByEmail',
                    title: 'Submitted By',
                    sortField: 'SubmittedByEmail',
                    filterable: true,
                    visible: true
                },
                {
                    name: 'Status',
                    title: 'Status',
                    sortField: 'Status',
                    filterable: true,
                    visible: true
                },
                {
                    name: 'StatusMessage',
                    title: 'Status Message',
                    sortField: 'StatusMessage',
                    filterable: true,
                    visible: true
                },
                {
                    name: 'actions',
                    title: '',
                    titleClass: 'center aligned',
                    dataClass: 'text-nowrap center aligned',
                    visible: true
                }
            ],
            courseSearchStatusTableSortOrder: [
                { field: 'LastUpdated', direction: 'asc' }
            ],
            courseSearchStatusTableCss: {
                table: {
                    tableClass: 'table table-striped table-bordered table-hovered',
                    loadingClass: 'loading',
                    ascendingIcon: 'fas fa-angle-up',
                    descendingIcon: 'fas fa-angle-down',
                    handleIcon: 'fas fa-bars'
                },
                pagination: {
                    infoClass: 'float-left',
                    wrapperClass: 'vuetable-pagination float-right',
                    activeClass: 'btn-primary',
                    disabledClass: 'disabled',
                    pageClass: 'btn btn-border',
                    linkClass: 'btn btn-border',
                    icons: {
                        first: 'fas fa-angle-double-left',
                        prev: 'fas fa-angle-left',
                        next: 'fas fa-angle-right',
                        last: 'fas fa-angle-double-right'
                    }
                }
            }
        },
        mounted: function () {

            axios.get("Home/GetEnrollmentTerms").then(response => {
                this.terms = response.data;

                this.termsOptions = [];
                response.data.forEach((object) => {
                    this.termsOptions.push(object);
                });
            });

            //axios.get('/Sample/GetSampleTypes')
            //    .then(response => {
            //        this.sampleTypes = response.data;
            //    });

            //axios.get('/Sample/GetSampleSources')
            //    .then(response => {
            //        this.sampleSources = response.data;
            //    });

            //axios.get('/Sample/GetReportTypes')
            //    .then(response => {
            //        this.reportTypes = response.data;
            //    });
        },
        methods: {
            queryParams: function (sortOrder, currentPage, perPage) {
                let sortOrderName = '';
                let sortOrderDirection = '';
                if (sortOrder !== undefined && sortOrder.length > 0) {
                    sortOrderName = sortOrder[0].sortField;
                    sortOrderDirection = sortOrder[0].direction;
                }

                return {
                    'sortName': sortOrderName,
                    'order': sortOrderDirection === 'asc' ? '' : 'descending',
                    'page': currentPage,
                    'per_page': perPage
                };
            },
            onPaginationData: function (paginationData) {
                this.$refs.pagination.setPaginationData(paginationData);
            },
            onChangePage: function (page) {
                this.$refs.vuetable.changePage(page);
            },
            runFilter: function () {
                this.filterText = JSON.stringify(this.filter);
                this.additionalParams = {
                    'filter': this.filterText
                };
                Vue.nextTick(() => this.$refs.vuetable.refresh());
            },
            resetFilter: function () {
                this.filter = cleanFilter();
                this.additionalParams = {};
                this.filterText = '';
                Vue.nextTick(() => this.$refs.vuetable.refresh());
            },
            addNewSearch: function () {
                var termsArray = [];
                
                this.termsValue.forEach(function(item) {
                    termsArray.push(item.Id);
                });

                var termsCsv = termsArray.toString();

                axios.post("Home/AddNewSearch", { TermList: termsCsv })
                    .then(response => {
                        location.reload();
                    });
            },
            showReport(search) {
                window.location.href = 'Report/UnusedCoursesReport?courseSearchQueueId=' + search.Id;
            }
        }
    });
});