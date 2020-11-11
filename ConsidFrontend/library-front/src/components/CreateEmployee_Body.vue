<template>
    <v-container>
        <form>
            <v-layout column>
                <v-flex class="display-2 text-xs-center my-5"
                    >Please fill all fields before pressing "Create"</v-flex
                >
                <v-flex>
                    <div>
                        <label for="first_name">First name: </label>
                        <input
                            class="input"
                            id="first_name"
                            type="text"
                            v-model="employee.first_name"
                            placeholder="First Name"
                        />
                        <label for="last_name">Last name: </label>
                        <input
                            class="input"
                            id="last_name"
                            type="text"
                            v-model="employee.last_name"
                            placeholder="Last Name"
                        /><br />
                        <label for="employee_type">Type of employee: </label>
                        <select v-model="selected_type" id="employee_type">
                            <option
                                v-for="(eT,index) in employee_types"
                                v-bind:key="eT"
                                v-bind:value="index"
                            >
                                {{ eT }}
                            </option>
                        </select>

                        <label for="manager_id" v-if="employee_type != 'CEO'"
                            >Type of employee:
                        </label>
                        <select
                            v-model="employee.manager_id"
                            id="manager_id"
                            v-if="employee_type != 'CEO'"
                        >
                            <option
                                v-for="employee in managers"
                                v-bind:key="employee.id"
                                v-bind:value="employee.id"
                            >
                                {{ employee.manager_id }}
                            </option>
                        </select>
                        <br>

                        <label for="rank">Rank of employee</label>
                        <input
                            id="rank"
                            type="number"
                            min="1"
                            max="10"
                            v-model="employee.rank"
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
            employee: {},
            manager_id: null
        };
    },
    mounted() {
        var requestOptions = {
            method: 'GET',
            redirect: 'follow'
        };
        var url = 'https://127.0.0.1:5001/employees/all';
        fetch(url, requestOptions)
            .then(response => response.json())
            .then(result => {
                console.log(result);
                this.managers = result.filter(
                    employee => employee.isCEO || employee.isManager
                );
            })
            .catch(error => console.log('error', error));
    },
    methods: {
        create: function() {
            var temp = document.getElementById('title');
            var title = temp == null ? null : temp.value;
            var author = null;
            var pages = null;
            var runTimeMinutes = null;
            if (
                this.selected_type == 'Book' ||
                this.selected_type == 'Reference Book'
            ) {
                temp = document.getElementById('author');
                if (temp == null || temp.value.length == 0) {
                    document.getElementById('author').style.backgroundColor =
                        'red';
                    return;
                } else
                    document.getElementById(
                        'author'
                    ).style.backgroundColor = null;
                author = temp.value;

                temp = document.getElementById('pages');
                if (temp == null || temp.value <= 0) {
                    document.getElementById('pages').style.backgroundColor =
                        'red';
                    return;
                } else
                    document.getElementById(
                        'pages'
                    ).style.backgroundColor = null;
                pages = temp.value;
            } else if (
                this.selected_type == 'DVD' ||
                this.selected_type == 'Audio Book'
            ) {
                temp = document.getElementById('runTimeMinutes');
                if (runTimeMinutes <= 0) {
                    console.log('RUNTIME FAILED');
                    document.getElementById(
                        'runTimeMinutes'
                    ).style.backgroundColor = 'red';
                    return;
                } else
                    document.getElementById(
                        'runTimeMinutes'
                    ).style.backgroundColor = null;
            } else {
                alert("Can't use invalid types");
                return;
            }

            //"check input" but realy just change color of input field and output info to log.
            //Possible that alert would be better, or something else, but alert is terrible when developing
            //Next time i will possibly use alert "if not HelperGlobalVariable.ISDEVELOPING is false"

            var myHeaders = new Headers();
            myHeaders.append('Content-Type', 'application/json');
            var obj = {};
            obj.title = title;
            obj.author = author;
            obj.pages = pages;
            obj.runTimeMinutes = runTimeMinutes;
            obj.type = this.selected_type;
            obj.categoryId = this.selected_category;
            obj.borrower = null;
            obj.borrowDate = null;
            obj.isBorrowable = obj.type == 'Reference Book' ? false : true;

            //Borrow won't change, nore will type, since "onChange"
            //var category = null;
            var raw = JSON.stringify(obj);
            console.log('Item to create is: ' + raw);
            raw = JSON.stringify(raw);
            console.log('Restringed item is: ' + raw);

            var requestOptions = {
                method: 'POST',
                headers: myHeaders,
                body: raw,
                redirect: 'follow'
            };
            var url = 'https://127.0.0.1:5001/library_item/';
            fetch(url, requestOptions)
                .then(response => response.json())
                .then(result => {
                    console.log(result);
                    console.log('Status var: ' + result.statusCode);
                    if (result.statusCode != 201) {
                        console.log('Will change colur');
                        alert(
                            'Failed updateing: Possibly will print error, but probably not, possibly to long input.\nOr lacking correct category'
                        );
                    } else {
                        alert('WILL reroute now!');
                        this.$router.push('/library_items');
                    }
                })
                .catch(error => console.log('HELVETES ERROR SKIT', error));
        }
    }
};
</script>

<style scoped></style>
