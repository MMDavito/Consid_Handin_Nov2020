<!--
Next time i do frontend i will use:
https://vuetifyjs.com/en/components/data-tables/
!-->
<template>
    <v-container>
        <v-layout column>
            <v-flex>
                <v-card>
                    <v-card-title>
                        Employees
                        <v-spacer></v-spacer>
                        <v-text-field
                            v-model="search"
                            append-icon="mdi-magnify"
                            label="Search"
                            single-line
                            hide-details
                        ></v-text-field>
                    </v-card-title>
                    <v-data-table
                        v-model="selected"
                        :headers="headers"
                        :items="employees"
                        :single-select="true"
                        item-key="id"
                        show-select
                        class="elevation-1"
                        :search="search"
                    ></v-data-table>
                </v-card>
                <!--
            <v-data-table
                v-model="selected"
                :headers="headers"
                :items="library_items"
                :single-select="true"
                item-key="id"
                show-select
                class="elevation-1"
            >
            </v-data-table>
!-->
                <v-btn color="primary" class="update" @click="edit()"
                    >Edit</v-btn
                >
                <v-btn color="error" class="delete" @click="remove()"
                    >Delete</v-btn
                >
            </v-flex>
        </v-layout>
    </v-container>
</template>

<script>
export default {
    name: 'EmployeesBody',
    data() {
        return {
            search: '',
            headers: [],
            selected: [],
            employees: []
        };
    },
    mounted() {
        var requestOptions = {
            method: 'GET',
            redirect: 'follow'
        };
        fetch('https://127.0.0.1:5001/employee/all', requestOptions)
            .then(response => response.json())
            .then(result => {
                console.log(result);
                this.employees = result;
                this.employees.forEach(function(v) {
                    delete v.listOfManaged;
                });

                var keys = Object.keys(this.employees[0]);
                keys.forEach(key => {
                    this.headers.push({ text: key, value: key });
                });
            })
            .catch(error => console.log('error', error));
    },
    methods: {
        remove: function() {
            if (this.selected.length == 0) {
                alert('Need to select an employee to edit');
                return;
            } //else:
            var id = this.selected[0].id;

            var r = confirm(
                'Do you want to delete: ' +
                    this.selected.firstName +
                    ', With id:' +
                    id
            );
            if (r == false) {
                alert('Will not remove since you canceled.');
                return;
            }
            var url = 'https://127.0.0.1:5001/employee/' + id;
            var requestOptions = {
                method: 'DELETE',
                redirect: 'follow'
            };
            fetch(url, requestOptions)
                .then(response => response.json())
                .then(result => {
                    console.log(result);
                    console.log('Status var: ' + result.statusCode);
                    if (result.statusCode != 200) {
                        console.log('Failed chekin in');
                        alert(
                            'Failed deleteing the employee with id: ' +
                                id +
                                "\nPossible employees are managed by this employee, try editing 'hen'"
                        );
                        return;
                    } else {
                        console.log();
                        var list_index = this.employees.findIndex(
                            i => i.id === id
                        );
                        this.library_items.splice(list_index, 1);
                    }
                });
        },
        edit: function() {
            if (this.selected.length == 0) {
                alert('Need to select an employee to edit');
                return;
            } //else:
            var id = this.selected[0].id;

            var ref = '/employee_edit/' + id;
            this.$router.push(ref);
        }
    }
};
</script>

<style scoped></style>
