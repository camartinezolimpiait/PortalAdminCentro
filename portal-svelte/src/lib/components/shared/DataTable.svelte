<!--
  DataTable.svelte
  Replaces: Shared/GridDinamico.razor
  DaisyUI table with pagination, loading state, and dynamic columns
-->
<script lang="ts">
	import type { Snippet } from 'svelte';

	let {
		data = [],
		columns = [],
		loading = false,
		currentPage = 1,
		totalPages = 1,
		onPageChange,
		emptyMessage = 'No se encontraron registros',
		children
	}: {
		data?: Record<string, unknown>[];
		columns?: { key: string; label: string; render?: (value: unknown, row: Record<string, unknown>) => string }[];
		loading?: boolean;
		currentPage?: number;
		totalPages?: number;
		onPageChange?: (page: number) => void;
		emptyMessage?: string;
		children?: Snippet;
	} = $props();

	function goToPage(page: number) {
		if (page >= 1 && page <= totalPages && onPageChange) {
			onPageChange(page);
		}
	}
</script>

{#if loading}
	<div class="flex justify-center items-center py-12">
		<span class="loading loading-spinner loading-lg text-primary"></span>
	</div>
{:else if data.length === 0}
	<div class="text-center py-12 text-base-content/60">
		<p>{emptyMessage}</p>
	</div>
{:else}
	<div class="overflow-x-auto">
		<table class="table table-zebra w-full">
			<thead>
				<tr>
					{#each columns as col}
						<th>{col.label}</th>
					{/each}
				</tr>
			</thead>
			<tbody>
				{#each data as row, i}
					<tr class="hover">
						{#each columns as col}
							<td>
								{#if col.render}
									{@html col.render(row[col.key], row)}
								{:else}
									{row[col.key] ?? '—'}
								{/if}
							</td>
						{/each}
					</tr>
				{/each}
			</tbody>
		</table>
	</div>

	{#if totalPages > 1}
		<div class="flex justify-center mt-4">
			<div class="join">
				<button
					class="join-item btn btn-sm"
					onclick={() => goToPage(currentPage - 1)}
					disabled={currentPage <= 1}
				>
					«
				</button>
				{#each Array.from({ length: Math.min(totalPages, 5) }, (_, i) => {
					const start = Math.max(1, currentPage - 2);
					return start + i;
				}).filter((p) => p <= totalPages) as page}
					<button
						class="join-item btn btn-sm {page === currentPage ? 'btn-active' : ''}"
						onclick={() => goToPage(page)}
					>
						{page}
					</button>
				{/each}
				<button
					class="join-item btn btn-sm"
					onclick={() => goToPage(currentPage + 1)}
					disabled={currentPage >= totalPages}
				>
					»
				</button>
			</div>
		</div>
	{/if}
{/if}
