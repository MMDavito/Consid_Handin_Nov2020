<template>
    <v-container>
        <form>
            <v-layout column>
                <v-flex class="display-2 text-xs-center my-5"
                    >Please fill all fields before pressing "Create"</v-flex
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
                        <v-btn color="primary" class="create" @click="create()"
                            >Create</v-btn
                        >
                        <br />
                    </div>
                </v-flex>
            </v-layout>
        </form>
    </v-container>
</template>

<script>
export default {
    name: 'CreateLibraryItem_Body',
    data() {
        return {
            selected_type: 'Employee',
            employee_types: ['Employee', 'Manager', 'CEO'],
            managers: [],
            employee: {
                firstName: '',
                lastName: '',
                rank: 1,
                managerId: null
            },
            managerId: null
        };
    },
    mounted() {
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
                    employee => employee.isCEO || employee.isManager
                );
                this.managers.push({});
                console.log('MANAGERS: ');
                console.log(this.managers);
            })
            .catch(error => console.log('error', error));
    },
    methods: {
        create: function() {
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

            //Now try to post:
            var myHeaders = new Headers();
            myHeaders.append('Content-Type', 'application/json');
            var raw = JSON.stringify(this.employee);
            console.log('Item to create is: ' + raw);
            raw = JSON.stringify(raw);
            console.log('Restringed item is: ' + raw);

            var requestOptions = {
                method: 'POST',
                headers: myHeaders,
                body: raw,
                redirect: 'follow'
            };
            var url = 'https://127.0.0.1:5001/employee/';
            fetch(url, requestOptions)
                .then(response => response.json())
                .then(result => {
                    console.log(result);
                    console.log('Status var: ' + result.statusCode);
                    if (result.statusCode != 201) {
                        console.log('Will change colur');
                        alert('Failed createing');
                        return;
                    } else {
                        alert('WILL reroute now!');
                        this.$router.push('/employees');
                    }
                })
                .catch(error => console.log('HELVETES ERROR SKIT', error));
        }
    }
};
</script>

<style scoped></style>
