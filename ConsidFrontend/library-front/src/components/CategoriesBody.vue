<template>
    <v-container>
        <v-layout column>
            <v-flex class="display-2 text-xs-center my-5"
                >Empty placeholder before delete</v-flex
            >
            <v-flex>
                <ul id="Categories">
                    <li v-for="(c, index) in categories" :key="c.category">
                        {{ c.category }} With id {{ c.id }} and index
                        {{ index }}
                    </li>
                </ul>
                <div v-for="(c, index) in categories" :key="c.category">
                    <label>{{ c.category }}</label
                    ><br />
                    <input
                        class="input"
                        :id="index"
                        type="text"
                        name="c.category"
                        :value="c.category"
                    />
                    <v-btn
                        color="primary"
                        class="update"
                        :name="index"
                        @click="update(index)"
                        >Update</v-btn
                    >
                    <v-btn
                        color="error"
                        class="delete"
                        :name="index"
                        @click="remove(index)"
                        >Delete</v-btn
                    >
                </div>
                <div>
                    <label>Add new category:</label><br />
                    <input
                        placeholder="Fill here"
                        class="input"
                        id="new_cat"
                        type="text"
                        name="new_cat"
                    />
                    <v-btn color="primary" class="add" @click="add()"
                        >Create</v-btn
                    >
                </div>
            </v-flex>
        </v-layout>
    </v-container>
</template>

<script>
export default {
    name: 'CategoriesBody',
    data() {
        return {
            categories: [],
            pokemon: []
        };
    },
    mounted() {
        var requestOptions = {
            method: 'GET',
            redirect: 'follow'
        };

        fetch('https://127.0.0.1:5001/category/all', requestOptions)
            .then(response => response.json())
            .then(result => {
                console.log(result);
                this.categories = result;
            })
            .catch(error => console.log('error', error));
    },
    methods: {
        update: function(clicked_id) {
            console.log('Clicked: ' + clicked_id);
            var input_field = document.getElementById(clicked_id);
            console.log(input_field);
            var newData = input_field.value;
            console.log(newData);
            console.log(this.categories);
            var tempCat = this.categories[clicked_id];
            console.log('New data: ' + newData);
            console.log('Old data: ' + tempCat.category);
            //"check input" but realy just change color of input field and output info to log.
            //Possible that alert would be better, or something else, but alert is terrible when developing
            //Next time i will possibly use alert "if not HelperGlobalVariable.ISDEVELOPING is false"
            if (
                newData == null ||
                newData == undefined ||
                !(newData.length > 0) ||
                newData.length > 200 ||
                newData == tempCat.category
            ) {
                console.log(
                    'New category cannot have an empty name or a name longer than 200 bytes.\nNore can it be the same as already existing'
                );
                document.getElementById(clicked_id).style.backgroundColor =
                    'red';
                return;
            } else if (tempCat.id == null) {
                //Following is caused by lazzyness when creating .net response api:
                document.getElementById(clicked_id).style.backgroundColor =
                    'red';
                alert(
                    'You must reload page before updateing newely created category.'
                );
                return;
            } else {
                document.getElementById(
                    clicked_id
                ).style.backgroundColor = null;
            }
            var myHeaders = new Headers();
            myHeaders.append('Content-Type', 'application/json');

            //var category = null;
            console.log(tempCat);
            tempCat.category = newData;
            console.log(tempCat);
            var raw = JSON.stringify(tempCat);
            console.log('New category is: ' + raw);
            raw = JSON.stringify(raw);
            console.log('New category is: ' + raw);

            var requestOptions = {
                method: 'PUT',
                headers: myHeaders,
                body: raw,
                redirect: 'follow'
            };
            var url = 'https://127.0.0.1:5001/category/' + tempCat.id;
            fetch(url, requestOptions)
                .then(response => response.json())
                .then(result => {
                    console.log(result);
                    console.log('Status var: ' + result.statusCode);
                    if (result.statusCode != 200) {
                        console.log('Will change colur');
                        console.log(document.getElementById(clicked_id));
                        document.getElementById(
                            clicked_id
                        ).style.backgroundColor = 'red';
                        console.log(document.getElementById(clicked_id));
                    } else {
                        this.$set(this.categories, clicked_id, {
                            id: tempCat.id,
                            category: tempCat.category
                        });
                        document.getElementById(
                            clicked_id
                        ).style.backgroundColor = null;
                    }
                })
                .catch(error => console.log('HELVETES ERROR SKIT', error));
        },
        add: function() {
            var input_field = document.getElementById('new_cat');
            var newData = input_field.value;
            if (
                newData == null ||
                newData == undefined ||
                !(newData.length > 0) ||
                newData.length > 200
            ) {
                console.log(
                    'New category cannot have an empty name or a name longer than 200 bytes.\nNore can it be the same as already existing'
                );
                document.getElementById('new_cat').style.backgroundColor =
                    'red';
                return;
            } else {
                document.getElementById('new_cat').style.backgroundColor = null;
            }

            var myHeaders = new Headers();
            myHeaders.append('Content-Type', 'application/json');
            var cat = { id: null, category: newData };
            console.log('Cat: ' + cat);
            var raw = JSON.stringify(cat);
            console.log('raw: ' + raw);
            raw = JSON.stringify(raw);

            console.log('raw: ' + raw);

            var requestOptions = {
                method: 'POST',
                headers: myHeaders,
                body: raw,
                redirect: 'follow'
            };

            fetch('https://127.0.0.1:5001/category', requestOptions)
                .then(response => response.text())
                .then(result => {
                    result = JSON.parse(result);
                    console.log(result);
                    console.log('STATUS FAN: ' + result.statusCode);
                    if (result.statusCode != 201) {
                        document.getElementById(
                            'new_cat'
                        ).style.backgroundColor = 'red';
                        return;
                    } else {
                        document.getElementById(
                            'new_cat'
                        ).style.backgroundColor = null;
                        this.categories.push(cat);
                    }
                })
                .catch(error => console.log('error', error));
        },
        remove: function(clicked_id) {
            var input_field = document.getElementById(clicked_id);
            console.log(input_field);
            var newData = input_field.value;
            console.log(newData);
            console.log(this.categories);
            var tempCat = this.categories[clicked_id];
            console.log('New data: ' + newData);
            console.log('Old data: ' + tempCat.category);
            //"check input" but realy just change color of input field and output info to log.
            //Possible that alert would be better, or something else, but alert is terrible when developing
            //Next time i will possibly use alert "if not HelperGlobalVariable.ISDEVELOPING is false"
            if (
                newData == null ||
                newData == undefined ||
                newData.length != 0
            ) {
                alert(
                    'To delete one must empty the textfield for the item one want to delete.\n\nThis is instead of confirmation.'
                );
                document.getElementById(clicked_id).style.backgroundColor =
                    'red';
                return;
            } else if (tempCat.id == null) {
                //Following is caused by lazzyness when creating .net response api:
                document.getElementById(clicked_id).style.backgroundColor =
                    'red';
                alert(
                    'You must reload page before deleteing newely created category.'
                );
                return;
            } else {
                document.getElementById(
                    clicked_id
                ).style.backgroundColor = null;
            }
            var requestOptions = {
                method: 'DELETE',
                redirect: 'follow'
            };
            var url = 'https://127.0.0.1:5001/category/' + tempCat.id;
            fetch(url, requestOptions)
                .then(response => response.text())
                .then(result => {
                    console.log(result);
                    result = JSON.parse(result);
                    if (result.statusCode != 200) {
                        document.getElementById(
                            clicked_id
                        ).style.backgroundColor = 'red';
                    } else {
                        this.categories.splice(clicked_id, 1);
                    }
                })
                .catch(error => console.log('error', error));
        }
    }
};
</script>

<style scoped></style>
