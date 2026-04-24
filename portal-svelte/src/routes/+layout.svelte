<script lang="ts">
import '../app.css';
import ToastContainer from '$lib/components/shared/ToastContainer.svelte';
import type { Snippet } from 'svelte';

let { children, data }: { children: Snippet; data: { user: import('$lib/types/models').SessionUser | null } } = $props();
</script>

<ToastContainer />
{@render children()}
