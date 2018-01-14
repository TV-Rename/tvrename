using System.Threading;

namespace TVRename.Core.Utility
{
    /// <summary>
    /// Thread-safe integer counter.
    /// </summary>
    public class Counter
    {
        private int value;

        /// <summary>
        /// Gets the counter value.
        /// </summary>
        /// <value>
        /// The counter value.
        /// </value>
        public int Value => Volatile.Read(ref this.value);

        /// <summary>
        /// Initializes a new instance of the <see cref="Counter"/> class.
        /// </summary>
        /// <param name="value">The initial counter value.</param>
        public Counter(int value) => this.value = value;

        /// <summary>
        /// Increments the counter value by one.
        /// </summary>
        /// <returns></returns>
        public void Increment() => Interlocked.Increment(ref this.value);

        /// <summary>
        /// Decrements the counter value by one.
        /// </summary>
        /// <returns></returns>
        public void Decrement() => Interlocked.Decrement(ref this.value);

        /// <summary>
        /// Clears the counter value.
        /// </summary>
        public void Clear() => this.value = 0;

        /// <summary>
        /// Returns a <see cref="T:string" /> that represents this instance counter value.
        /// </summary>
        /// <returns>
        /// A <see cref="T:string" /> that represents this instance counter value.
        /// </returns>
        public override string ToString() => this.Value.ToString();
    }
}
