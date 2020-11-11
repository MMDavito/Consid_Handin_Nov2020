<template>
    <v-container>
        <form>
            <v-layout column>
                <v-flex class="display-2 text-xs-center my-5"
                    >Please fill all fields before pressing "Update"</v-flex
                >
                <v-flex>
                    <div>
                        <label for="firstName">First name: </label>
                        <input
                            class="input"
                            id="firstName"
                            type="text"
                            v-model="employee.firstName"
                            placeholder="First Name"
                        />
                        <label for="lastName">Last name: </label>
                        <input
                            class="input"
                            id="lastName"
                            type="text"
                            v-model="employee.lastName"
                            placeholder="Last Name"
                        /><br />
                        <label for="employee_type">Type of employee: </label>
                        <select
                            v-model="selected_type"
                            id="employee_type"
                            style="border-style:solid;"
                        >
                            <option
                                v-for="eT in employee_types"
                                v-bind:key="eT"
                                v-bind:value="eT"
                            >
                                {{ eT }}
                            </option>
                        </select>
                        <br />
                        <label for="managerId" v-if="selected_type != 'CEO'"
                            >Is managed by (last option is managed by none):
                        </label>
                        <select
                            v-model="employee.managerId"
                            id="managerId"
                            v-if="selected_type != 'CEO'"
                            style="border-style:solid;"
                        >
                            <option
                                v-for="tE in managers"
                                v-bind:key="tE.id"
                                v-bind:value="tE.id"
                            >
                                {{ tE.firstName }}
                            </option>
                        </select>
                        <br />

                        <label for="rank"
                            >Rank of employee (in range: [1,10]):
                        </label>
                        <input
                            id="rank"
                            type="number"
                            min="1"
                            max="10"
                            v-model="employee.rank"
                            style="border-style:solid;"
                        />

                        <br />
                        <v-btn color="primary" class="create" @click="update()"
                            >Update</v-btn
                        >
                        <v-btn
                            color="error"
                            class="create"
                            @click="removeThis()"
                            >Remove This</v-btn
                        >
                        <br />
                    </div>
                </v-flex>
            </v-layout>
        </form>

        <v-card>
            <v-card-title>
                Managed by user:
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
                :items="listOfManaged"
                :single-select="true"
                item-key="id"
                show-select
                class="elevation-1"
                :search="search"
            ></v-data-table>
        </v-card>
        <v-btn color="primary" class="update" @click="edit()">Edit</v-btn>
        <v-btn color="error" class="delete" @click="remove()">Delete</v-btn>
        <br />
        <br />
        <v-btn color="error" class="delete" @click="removeAll()"
            >Delete ALL that is managed by {{ employee.firstName }}</v-btn
        >
    </v-container>
</template>

<script>
export default {
    name: 'EditeEmployeeBody',
    data() {
        return {
            selected_type: 'Employee',
            employee_types: ['Employee', 'Manager', 'CEO'],
            managers: [],
            listOfManaged: [],
            employee: {
                firstName: '',
                lastName: '',
                rank: 1,
                managerId: null
            },
            managerId: null,
            search: '',
            headers: [],
            selected: []
        };
    },
    mounted() {
        this.employee.id = this.$route.params.id;

        var requestOptions = {
            method: 'GET',
            redirect: 'follow'
        };
        var url = 'https://127.0.0.1:5001/employee/all';
        fetch(url, requestOptions)
            .then(response => response.json())
            .then(result => {
                console.log(result);
                this.managers = result.filter(
                    employee =>
                        employee.isCEO ||
                        employee.isManager ||
                        employee.id == this.employeeid
                );
                this.managers.push({});
                console.log('MANAGERS: ');
                console.log(this.managers);
            })
            .catch(error => console.log('error', error));
        url = 'https://127.0.0.1:5001/employee/' + this.employee.id;
        fetch(url, requestOptions)
            .then(response => response.json())
            .then(result => {
                result = result[0];
                console.log(result);
                this.listOfManaged = result.listOfManaged;
                if (this.listOfManaged.length > 0) {
                    var keys = Object.keys(this.listOfManaged[0]);
                    keys.forEach(key => {
                        this.headers.push({ text: key, value: key });
                    });
                }
                console.log('LIST OF MANAGED: ');
                console.log(this.listOfManaged);
                delete result.listOfManaged;
                if (result.isCEO) this.selected_type = 'CEO';
                else if (result.isManager) this.selected_type = 'Manager';
                else this.selected_type = 'Employee';
                this.employee = result;
            });
    },
    methods: {
        update: function() {
            if (
                this.employee.rank == null ||
                this.employee.rank < 1 ||
                this.employee.rank > 10
            ) {
                document.getElementById('rank').style.backgroundColor = 'red';
                return;
            } else document.getElementById('rank').style.backgroundColor = null;

            console.log('managerid:' + this.employee.managerId);
            if (
                this.selected_type == 'Employee' &&
                this.employee.managerId == null
            ) {
                alert('Employee must have a manager');
                return;
            }
            console.log('First name: ' + this.employee.firstName);
            console.log('last name: ' + this.employee.lastName);
            console.log('First name length: ' + this.employee.firstName.length);
            console.log('last name length: ' + this.employee.lastName.length);

            if (
                this.employee.firstName.length == 0 ||
                this.employee.lastName.length == 0
            ) {
                alert('Employee must have a first and last name');
                return;
            }
            console.log('JAG ÄR EJ FÖRTVIVLAD');
            this.employee.isCEO = this.selected_type == 'CEO';
            this.employee.isManager = this.selected_type == 'Manager';

            //Now try to put:
            var myHeaders = new Headers();
            myHeaders.append('Content-Type', 'application/json');
            var raw = JSON.stringify(this.employee);
            console.log('Item to create is: ' + raw);
            raw = JSON.stringify(raw);
            console.log('Restringed item is: ' + raw);

            var requestOptions = {
                method: 'PUT',
                headers: myHeaders,
                body: raw,
                redirect: 'follow'
            };
            var url = 'https://127.0.0.1:5001/employee/' + this.employee.id;
            fetch(url, requestOptions)
                .then(response => response.json())
                .then(result => {
                    console.log(result);
                    console.log('Status var: ' + result.statusCode);
                    if (result.statusCode != 200) {
                        console.log('Will change colur');
                        alert('Failed update');
                        return;
                    } else {
                        alert('WILL reroute now!');
                        this.$router.push('/employees');
                    }
                })
                .catch(error => console.log('HELVETES ERROR SKIT', error));
        },
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
                        var list_index = this.listOfManaged.findIndex(
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

            //            var ref = '/employee_edit/' + id;
            //this.$set('this.$route.params', 'id', id);
            //this.$router.push(ref);
            this.$router.push({ name: 'EmployeeEdit', params: { id: id } });

            //this.mounted()
        },
        removeAll: function() {
            var id = this.employee.id;

            var r = confirm(
                'Do you want to delete employees managed by: ' +
                    this.selected.firstName +
                    ', With id:' +
                    id
            );
            if (r == false) {
                alert('Will not remove since you canceled.');
                return;
            }
            var url = 'https://127.0.0.1:5001/employees/managed_by/' + id;
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
                        console.log('Failed deleteing');
                        alert(
                            'Failed deleteing the employees managed by: ' +
                                id +
                                '\nPossible employees manages other employees, try editing them one by one.'
                        );
                        return;
                    } else {
                        console.log();
                        this.$set(this, 'listOfManaged', []);
                    }
                });
        },
        removeThis: function() {
            var id = this.employee.id;

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
                        console.log('Failed deleteing');
                        alert(
                            'Failed deleteing the employee: ' +
                                id +
                                '\nPossible this employee manages other employees, try editing them one by one.'
                        );
                        return;
                    } else {
                        this.$router.push('/employees');
                    }
                });
        }
    }
};
</script>

<style scoped></style>
