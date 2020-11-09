<template>
    <v-container>
        <v-layout column>
            <v-flex class="display-2 text-xs-center my-5"
                >Big Title Goes Here</v-flex
            >
            <v-flex>
                <ul id="LibraryItems">
                    <li v-for="(l, index) in library_items" :key="l.title">
                        {{ l.title }} With id {{ l.id }} and index:
                        {{ index }}
                    </li>
                </ul>
                <div v-for="(l, index) in library_items" :key="l.title">
                    <label>{{ l.title }} ({{ l.acro }})</label><br />
                    <label :for="'borrower_' + index">Borrowed by: </label>
                    <input
                        class="input"
                        :id="'borrower_' + index"
                        type="text"
                        name="l.title"
                        :value="l.borrower"
                        placeholder="BORROWED BY"
                    />
                    <label :for="'borrow_date_' + index">Date borrowed: </label>
                    <input
                        class="input"
                        :id="'borrow_date_' + index"
                        type="date"
                        name="l.title"
                        :value="l.borrow_date"
                    />
                    <v-btn
                        v-if="l.type != 'Reference Book' && l.isBorrowable"
                        color="primary"
                        class="update"
                        :name="index"
                        @click="check_in(index)"
                        >Check In</v-btn
                    >

                    <v-btn
                        v-if="l.type != 'Reference Book' && !l.isBorrowable"
                        color="error"
                        class="update"
                        :name="index"
                        @click="check_out(index)"
                        >Check Out</v-btn
                    >
                    <br />
                    <v-btn
                        color="primary"
                        class="update"
                        :name="index"
                        @click="edit(index)"
                        >Edit</v-btn
                    >
                    <v-btn
                        color="error"
                        class="delete"
                        :name="index"
                        @click="remove(index)"
                        >Delete</v-btn
                    >
                    <br />
                </div>
            </v-flex>
        </v-layout>
    </v-container>
</template>

<script>
export default {
    name: 'LibraryItemBody',
    data() {
        return {
            library_items: [],
            pokemon: []
        };
    },
    mounted() {
        var requestOptions = {
            method: 'GET',
            redirect: 'follow'
        };

        fetch('https://127.0.0.1:5001/library_item/all', requestOptions)
            .then(response => response.json())
            .then(result => {
                console.log(result);
                this.library_items = result;

                this.library_items.forEach(element => {
                    element.acro = this.getAcronym(element.title);
                    console.log('Element after acronyminisisis:');
                    console.log(element);
                });
            })
            .catch(error => console.log('error', error));
    },
    methods: {
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
        },
        edit: function(clicked_id) {
            var id = this.library_items[clicked_id].id;
            var ref = 'library_items/' + id;
            this.$router.push(ref);
        },
        getAcronym(name) {
            name = name.toUpperCase();
            return name
                .split(/\s/)
                .reduce(
                    (accumulator, word) => accumulator + word.charAt(0),
                    ''
                );
        },
        check_in: function(list_index) {
            console.log('Index clicked: ' + list_index);
            var item = this.library_items[list_index];
            console.log('Item is: ' + item.title);
            var tempId = item.id;
            console.log('Item id is: ' + tempId);
            console.log('Item type is: ' + item.type);
            console.log('Item from list type is: ' + this.library_items[list_index].type);


            var tempInput = document.getElementById('borrower_' + list_index);
            var borrower = tempInput.value;
            if (borrower == null || borrower.length == 0) {
                document.getElementById(
                    'borrower_' + list_index
                ).style.backgroundColor = 'red';
                return;
            } else {
                document.getElementById(
                    'borrower_' + list_index
                ).style.backgroundColor = null;
            }
            tempInput = document.getElementById('borrow_date_' + list_index);
            var borrower_date = tempInput.value;
            if (borrower_date == null) {
                document.getElementById(
                    'borrow_date_' + list_index
                ).style.backgroundColor = 'red';
                return;
            } else {
                document.getElementById(
                    'borrow_date_' + list_index
                ).style.backgroundColor = null;
            }

            item.borrower = borrower;
            item.borrowDate = borrower_date;
            var raw = JSON.stringify(item);
            console.log('RawForCheckin: ' + raw);
            raw = JSON.stringify(raw);
            console.log('RawerForCheckin: ' + raw);

            var myHeaders = new Headers();
            myHeaders.append('Content-Type', 'application/json');
            var requestOptions = {
                method: 'PUT',
                headers: myHeaders,
                body: raw,
                redirect: 'follow'
            };
            var url = 'https://127.0.0.1:5001/library_item/check_in/' + tempId;
            fetch(url, requestOptions)
                .then(response => response.json())
                .then(result => {
                    console.log(result);
                    console.log('Status var: ' + result.statusCode);
                    if (result.statusCode != 200) {
                        console.log('Failed chekin in');
                        alert(
                            'Failed chekin in, would give reason, but notime to translate response, and bad to give user verbose server info.'
                        );
                    } else {
                        this.$set(this.library_items, list_index, {
                            item
                        });
                    }
                })
                .catch(error => console.log('HELVETES ERROR SKIT', error));
        },
        check_out: function(list_index) {
            alert(
                'Will check out: ' +
                    list_index +
                    '\n' +
                    this.library_items[list_index]
            );
        }
    }
};
</script>

<style scoped></style>
