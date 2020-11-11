<template>
    <span>
        <employee-edit-body :key="$route.fullPath"></employee-edit-body>
    </span>
</template>

<script>
import EmployeeEditBody from '@/components/EmployeeEditBody';

export default {
    name: 'edit_employee',
    components: {
        EmployeeEditBody
    }
};
</script>
