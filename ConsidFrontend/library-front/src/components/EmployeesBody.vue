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
                    :items="library_items"
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
                     <v-btn
                        color="primary"
                        class="update"
                        :name="index"
                        @click="edit()"
                        >Edit</v-btn
                    >
                    <v-btn
                        color="error"
                        class="delete"
                        :name="index"
                        @click="remove()"
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
            library_items: [],
            categories: [],
            sortByCategory: true //Should check if browser allows and have value, but NO TIME
        };
    },
    mounted() {
        var requestOptions = {
            method: 'GET',
            redirect: 'follow'
        };
        //Should have used a sql join on the server, but forgot, probably since i was not involved in the databaseDesign
        fetch('https://127.0.0.1:5001/category/all', requestOptions)
            .then(response => response.json())
            .then(result => {
                console.log(result);
                this.categories = result;
                if (this.library_items.length == 0) {
                    console.log('Library items is null, cant change them');
                    return;
                }
                //Else do
                //this.library_items.forEach(element => {});
            })
            .catch(error => console.log('error', error));

        fetch('https://127.0.0.1:5001/library_item/all', requestOptions)
            .then(response => response.json())
            .then(result => {
                console.log(result);
                this.library_items = result;

                this.library_items.forEach(element => {
                    console.log(
                        'Element has categoryId: ' + element.categoryId
                    );
                    if (this.categories.length == 0) {
                        console.log(
                            'Categories is null, cant querrry for them, try refresh page'
                        );
                        return;
                    }
                    var tempCat = this.categories.find(
                        category => category.id == element.categoryId
                    ); //Time complexity would be lower if returned an joined result from database or used hashset here
                    console.log(
                        'Element ' +
                            element.title +
                            ', Has category: ' +
                            tempCat.category
                    );
                    element.category = tempCat;
                    console.log(
                        'Element now have category: ' +
                            element.category.category
                    );

                    element.acro = this.getAcronym(element.title);

                    if (element.borrowDate != null) {
                        element.borrowDate = element.borrowDate.split('T')[0];
                    }
                    console.log('Element after acronyminisisis:');
                    console.log(element);
                });
                var keys = Object.keys(this.library_items[0]);
                keys.forEach(key => {
                    this.headers.push({ text: key, value: key });
                });

                console.log(
                    'Should i sort by category?: ' + this.sortByCategory
                );
                var sortBol =
                    sessionStorage.isCategorySort == 'true' ? true : false;
                this.$set(this, 'sortByCategory', sortBol);
                //this.sortByCategory=sortBol;
                if (this.sortByCategory) {
                    this.library_items.sort(this.compare_categories);
                } else {
                    this.library_items.sort(this.compare_types);
                }
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
            var ref = '/library_item_edit/' + id;
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
