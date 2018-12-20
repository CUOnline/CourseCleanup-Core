Vue.use(Vuetable);

Array.prototype.any = function (callback) { return this.findIndex(callback) > -1; };

$(document).ready(function () {
    var app = new Vue({
        el: "#app",
        data: {
            deletedCoursesReportTableFields: [
                {
                    name: 'Term',
                    title: 'Term',
                    sortField: 'Term',
                    filterable: true,
                    visible: true
                },
                {
                    name: 'LastUpdated',
                    title: 'Deleted',
                    sortField: 'LastUpdated',
                    filterable: true,
                    visible: true,
                    formatter: (value) => {
                        if (!value) return "";
                        return moment(value).format('M/D/YYYY');
                    }
                },
                {
                    name: 'CourseId',
                    title: 'Course Id',
                    sortField: 'CourseId',
                    filterable: true,
                    visible: true
                },
                {
                    name: 'CourseName',
                    title: 'Course Name',
                    sortField: 'CourseName',
                    filterable: true,
                    visible: true
                },
                {
                    name: 'CourseSISID',
                    title: 'Course SIS ID',
                    sortField: 'CourseSISID',
                    filterable: true,
                    visible: true
                },
                {
                    name: 'CourseCode',
                    title: 'Course Code',
                    sortField: 'CourseCode',
                    filterable: true,
                    visible: true
                },
                {
                    name: 'actions',
                    title: '',
                    titleClass: 'center aligned',
                    dataClass: 'text-nowrap center aligned',
                    sortField: 'Status',
                    visible: true
                }
            ],
            deletedCoursesReportTableSortOrder: [
                { field: 'LastUpdated', direction: 'asc' }
            ],
            deletedCoursesReportTableCss: {
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
        created: function () {
        },
        mounted: function () {
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
            confirmReactivate: function (rowData) {
                axios.get("/Report/ReactivateCourse/" + rowData.Id).then(response => {
                    this.$refs.vuetable.reload();
                });
            }
        }
    });
});