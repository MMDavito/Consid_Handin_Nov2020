<template>
    <v-container>
        <v-layout column>
            <v-flex class="display-2 text-xs-center my-5"
                >Big Title Goes Here</v-flex
            >
            <v-flex>
                <ul id="LibraryItems">
                    <li v-for="(l, index) in library_items" :key="l.id">
                        {{ l.title }} With id {{ l.id }} and index:
                        {{ index }}
                    </li>
                </ul>
                <div></div>
                <div v-for="(l, index) in library_items" :key="l.id">
                    <label
                        >id:{{ l.id }}, Title: {{ l.title }} ({{
                            l.acro
                        }}),</label
                    >
                    <label
                        v-if="
                            library_item.type == 'Book' ||
                                library_item.type == 'Reference Book'
                        "
                        >Author: {{ l.author }}, Pages: {{ l.pages }},</label
                    >
                    <label
                        v-if="
                            library_item.type == 'DVD' ||
                                library_item.type == 'Audio Book'
                        "
                        >Run time: {{ l.runTimeMinutes }},</label
                    >
                    <label>Type:{{ l.type }}, Category:{{categories.l.categoryId}}</label>

                    <br />

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
                        :value="l.borrowDate"
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
            categories: {}
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

                    if (element.borrowDate != null) {
                        element.borrowDate = element.borrowDate.split('T')[0];
                    }
                    console.log('Element after acronyminisisis:');
                    console.log(element);
                });/*TODO WILL NEED TO DO SO THIS GIVES CATEGORY: 
                categories.l.categoryId 
                It would really be easier to return it from backend, but i dont want unneccecerry verbose repetition of data from server.
                This verbose is bad comes from programming Arduino. I will in future do verbose from backend?
                
                Anyways this should be faster than the loop for every library_item.
                */
            })
            .catch(error => console.log('error', error));
    },
    methods: {
        remove: function(list_index) {
            var r = confirm(
                'Do you want to delete: ' +
                    this.library_items[list_index].title +
                    ', With id:' +
                    this.library_items[list_index].id
            );
            if (r == false) {
                alert('Will not remove since you canceled.');
                return;
            }
            var tempId = this.library_items[list_index].id;
            var url = 'https://127.0.0.1:5001/library_item/' + tempId;
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
                            'Failed chekin in, would give reason, but notime to translate response, and bad to give user verbose server info.'
                        );
                    } else {
                        console.log();
                        this.library_items.splice(list_index, 1);
                    }
                });
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
            console.log(
                'Item from list type is: ' + this.library_items[list_index].type
            );

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
                        //item.borrowDate = item.borrowDate.split('T')[0];
                        console.log(
                            'This new have borrower?: ' + item.borrower
                        );
                        /*
                        this.$set(this.library_items, list_index, {
                            item
                        });*/
                    }
                })
                .catch(error => console.log('HELVETES ERROR SKIT', error));
        },
        check_out: function(list_index) {
            var tempId = this.library_items[list_index].id;
            var url = 'https://127.0.0.1:5001/library_item/check_out/' + tempId;
            var requestOptions = {
                method: 'PUT',
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
                            'Failed chekin in, would give reason, but notime to translate response, and bad to give user verbose server info.'
                        );
                    } else {
                        //item.borrowDate = item.borrowDate.split('T')[0];
                        console.log();
                        var item = this.library_items[list_index];
                        item.borrower = null;
                        item.borrowDate = null;
                        item.isBorrowable = true;
                        this.$set(this.library_items, list_index, item);
                    }
                });
        }
    }
};
</script>

<style scoped></style>
