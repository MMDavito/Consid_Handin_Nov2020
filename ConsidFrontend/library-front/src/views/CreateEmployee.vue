<template>
    <span>
        <create-employee-body></create-employee-body>
    </span>
</template>

<script>
import CreateEmployeeBody from '@/components/CreateEmployee_Body';

export default {
    name: 'create_employee',
    components: {
        CreateEmployeeBody
    }
};
</script>
