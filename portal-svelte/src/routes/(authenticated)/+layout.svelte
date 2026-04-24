<!--
  (authenticated)/+layout.svelte
  Wraps all authenticated pages with PortalLayout
  Replaces: @layout PortalLayout in Blazor pages
-->
<script lang="ts">
	import PortalLayout from '$lib/components/layout/PortalLayout.svelte';
	import type { Snippet } from 'svelte';

	let { data, children }: { data: { user: import('$lib/types/models').SessionUser | null }; children: Snippet } = $props();
</script>

<PortalLayout user={data.user}>
	{@render children()}
</PortalLayout>
