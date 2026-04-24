# Style Migration Guide: Blazor → DaisyUI

## Component Mapping

### Blazor/Bootstrap → DaisyUI

| Original Component | DaisyUI Equivalent | Classes Used |
|---|---|---|
| Bootstrap `btn btn-primary` | DaisyUI button | `btn btn-primary` |
| Bootstrap `btn btn-outline-*` | DaisyUI outline button | `btn btn-outline` |
| Bootstrap `alert alert-*` | DaisyUI alert | `alert alert-success/error/warning/info` |
| Bootstrap `modal` | DaisyUI modal | `modal`, `modal-box`, `modal-action` |
| Bootstrap `dropdown` | DaisyUI dropdown | `dropdown`, `dropdown-content` |
| Bootstrap `table` | DaisyUI table | `table`, `table-zebra` |
| Bootstrap `form-control` | DaisyUI form-control | `form-control`, `label`, `input` |
| Bootstrap `card` | DaisyUI card | `card`, `card-body`, `card-title` |
| Bootstrap `nav` | DaisyUI menu/navbar | `navbar`, `menu`, `menu-horizontal` |
| Bootstrap `tab` | DaisyUI tabs | `tabs`, `tab`, `tab-active` |
| BlazorPro.SpinKit | DaisyUI loading | `loading loading-spinner` |
| Blazored.Toast | Custom toast store | `toast`, `alert` components |
| Bootstrap `badge` | DaisyUI badge | `badge badge-*` |
| Custom CSS sidebar | DaisyUI drawer | `drawer`, `drawer-side`, `drawer-content` |

### Business State Visual Mapping

| Business State | Original Style | DaisyUI Class | Accessibility |
|---|---|---|---|
| Alerta temprana (Early warning) | Custom CSS red/orange | `state-alerta-temprana` → `bg-warning text-warning-content` | WCAG AA contrast ✓ |
| Irregularidad (Irregularity) | Custom CSS red | `state-irregularidad` → `bg-error text-error-content` | WCAG AA contrast ✓ |
| Sin SOAT (No SOAT) | Custom red border | `state-sin-soat` → `bg-error/20 text-error border-error` | High visibility ✓ |
| Sin RTMyEC vigente | Custom orange border | `state-sin-rtmyec` → `bg-warning/20 text-warning border-warning` | High visibility ✓ |
| Vigente (Active/Valid) | Custom green | `state-vigente` → `bg-success/20 text-success border-success` | ✓ |
| Vencido (Expired) | Custom red | `state-vencido` → `bg-error/20 text-error border-error` | ✓ |

### Theme Configuration

```css
/* DaisyUI themes used */
themes: ['light', 'dark']

/* Custom CSS variables preserved */
--color-azul-600: #1e40af;    /* Primary brand blue */
--color-rojo-alerta: #dc2626; /* Error/alert red */
--color-verde-ok: #16a34a;    /* Success green */
--color-naranja-warn: #ea580c; /* Warning orange */
```

### Accessibility Criteria Applied
1. **Focus Visible**: All interactive elements have `:focus-visible` outline
2. **Contrast**: DaisyUI semantic colors meet WCAG AA minimum contrast ratios
3. **Keyboard Navigation**: All menus, modals, and forms are keyboard accessible
4. **Mobile-First**: All layouts start from mobile breakpoints using Tailwind responsive prefixes
5. **Label Association**: Form labels use `for` attributes linked to input `id`s
6. **ARIA**: Roles applied to tabs, modals, navigation elements
7. **Loading States**: Loading spinners use DaisyUI loading classes with ARIA attributes

### Layout Migration

| Blazor Layout | SvelteKit Implementation |
|---|---|
| `PortalLayout.razor` (root admin) | `PortalLayout.svelte` - DaisyUI drawer + navbar |
| `LoginLayout.razor` (login) | Standalone layout in login page - centered card |
| `SingleLayout.razor` | `(authenticated)/+layout.svelte` wrapping with PortalLayout |
| `TwoPaneLayout.razor` | Grid layout within page components |
| `SupertransporteLayout.razor` | Page-level layout with back navigation |
| `FormularioLayout.razor` | Page-level layout with back button and form wrapper |
| `PowerBILayout.razor` | Integrated in powerbi page |

### Responsive Breakpoints
| Breakpoint | Usage |
|---|---|
| `sm:` (640px) | Show additional table columns, expand horizontal tabs |
| `md:` (768px) | Switch to 2-column form grids, show sidebar navigation |
| `lg:` (1024px) | Full desktop layout with permanent drawer sidebar |
| `xl:` (1280px) | Extended content width |
